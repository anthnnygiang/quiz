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

        foreach (var card in cards)
        {
            card.Ask();
        }

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
            var cards = JsonSerializer.Deserialize<List<Card>>(
                File.ReadAllText(filePath),
                JsonOptions);
            
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
            AnsiConsole.MarkupLine($"Loaded {cards.Count} cards");
            return cards;
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to load quiz file:[/] {ex.Message.EscapeMarkup()}");
            return null;
        }
    }
}