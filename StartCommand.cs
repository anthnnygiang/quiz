using System.ComponentModel;
using System.Text.Json;

using Spectre.Console;
using Spectre.Console.Cli;

namespace Quiz;

public class StartCommand : Command<StartCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<file.json>")]
        [Description("The quiz file")]
        public required string QuizFile { get; init; }

        [CommandOption("-s|--shuffle")]
        [Description("Shuffle the quiz questions")]
        public bool Shuffle { get; init; }
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly Random Rng = new();

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        const string quizDirectory = ".quiz";
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), quizDirectory,
            settings.QuizFile);
        var cards = LoadQuizFile(filePath);
        if (cards is null)
        {
            return 1;
        }
        if (settings.Shuffle)
        {
            ShuffleList(cards);
        }

        var correctAnswers = 0;
        var incorrectAnswers = 0;

        for (var i = 0; i < cards.Count; i++)
        {
            var questionNumber = $"[dim]{i + 1}.[/]";
            var response = AnsiConsole.Ask<string>($"{questionNumber} {cards[i].Question.EscapeMarkup()} ");
            var match = cards[i].CheckAnswer(response);
            AnsiConsole.Write("\e[1A\e[2K"); // Clear the previous line
            string icon;
            string ending;
            switch (match)
            {
                case AnswerMatch.Correct:
                    icon = "[palegreen1](✔)[/]";
                    ending = string.Empty;
                    correctAnswers++;
                    break;
                case AnswerMatch.Incorrect:
                    icon = "[orangered1](✗)[/]";
                    ending = $"[dim] {cards[i].Answer.EscapeMarkup()}[/]";
                    incorrectAnswers++;
                    break;
                default:
                    icon = string.Empty;
                    ending = string.Empty;
                    break;
            }

            AnsiConsole.MarkupLine(
                $"{questionNumber} {cards[i].Question.EscapeMarkup()} {response.EscapeMarkup()} {icon}{ending}");
        }

        AnsiConsole.MarkupLine($"[dim]Results[/]");
        var breakdownChart = new BreakdownChart()
            .AddItem("Correct", correctAnswers, Color.PaleGreen1)
            .AddItem("Incorrect", incorrectAnswers, Color.OrangeRed1);
        AnsiConsole.Write(breakdownChart);
        return 0;
    }

    private static List<Card>? LoadQuizFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            AnsiConsole.MarkupLine($"[red]Quiz file not found:[/] {filePath.EscapeMarkup()}");
            return null;
        }

        if (!Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
        {
            AnsiConsole.MarkupLine(
                $"[red]Invalid quiz file:[/] {filePath.EscapeMarkup()} [grey](expected a .json file)[/]");
            return null;
        }

        try
        {
            using var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read
            );
            var cards = JsonSerializer.Deserialize<List<Card>>(stream, JsonOptions);

            // file contains 'null' or '[]'
            if (cards is null || cards.Count == 0)
            {
                AnsiConsole.MarkupLine($"[red]Quiz file is empty:[/] {filePath.EscapeMarkup()}");
                return null;
            }

            // All cards must have non-empty question, answer, and culture properties
            if (cards.Any(card => string.IsNullOrWhiteSpace(card.Question) ||
                                  string.IsNullOrWhiteSpace(card.Answer) ||
                                  string.IsNullOrWhiteSpace(card.Culture)))
            {
                AnsiConsole.MarkupLine($"[red]Quiz file contains invalid quiz items:[/] {filePath.EscapeMarkup()}");
                return null;
            }

            AnsiConsole.MarkupLine($"[dim]{cards.Count} question(s)[/]");
            return cards;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to load quiz file:[/] {ex.Message.EscapeMarkup()}");
            return null;
        }

    }
    // Fisher-Yates shuffle algorithm
    public static void ShuffleList<T>(IList<T> list)
    {
        var n = list.Count;
        for (var i = n - 1; i > 0; i--)
        {
            var j = Rng.Next(i + 1); // random index from 0 to i
            (list[i], list[j]) = (list[j], list[i]); // swap cards[i] with cards[j]
        }
    }
}
