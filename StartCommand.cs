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
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

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

        var correctAnswers = 0;
        var incorrectAnswers = 0;

        for (var i = 0; i < cards.Count; i++)
        {
            // AnsiConsole.Markup($"[grey]{i + 1}. [/]");
            var questionNumber = $"[grey]{i + 1}.[/]";
            var response = AnsiConsole.Ask<string>($"{questionNumber} {cards[i].Question.EscapeMarkup()} ");
            var match = cards[i].CheckAnswer(response);
            AnsiConsole.Write("\e[1A\e[2K"); // Clear the previous line
            string icon;
            switch (match)
            {
                case AnswerMatch.Correct:
                    icon = "[green1](✔)[/]";
                    correctAnswers++;
                    break;
                case AnswerMatch.Incorrect:
                    icon = "[red1](✗)[/]";
                    incorrectAnswers++;
                    break;
                default:
                    icon = string.Empty;
                    break;
            }

            AnsiConsole.MarkupLine(
                $"{questionNumber} {cards[i].Question.EscapeMarkup()} {response.EscapeMarkup()} {icon}");
        }

        AnsiConsole.MarkupLine($"Results:");
        var breakdownChart = new BreakdownChart()
            .AddItem("Correct", correctAnswers, Color.Green1)
            .AddItem("Incorrect", incorrectAnswers, Color.Red1);
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

            AnsiConsole.MarkupLine($"[cyan]{cards.Count} question(s)[/]");
            return cards;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to load quiz file:[/] {ex.Message.EscapeMarkup()}");
            return null;
        }
    }
}