using Spectre.Console;
using Spectre.Console.Cli;

namespace Learn;

public class Program
{
    static void Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config => { config.AddCommand<ListCommand>("list"); });
        app.Run(args);

        // var questions = new[]
        // {
        //     new QuestionAnswer("What is the capital of France?", "paris"),
        //     new QuestionAnswer("What is the largest planet in the solar system?", "Jupiter"),
        //     new QuestionAnswer("What is the chemical symbol for gold?", "Au"),
        //     new QuestionAnswer("What is the square root of 16?", "4"),
        //     // Future examples:
        //     // new Question("Tiếng Việt: thủ đô của Việt Nam là gì?", "Hà Nội", new System.Globalization.CultureInfo("vi-VN")),
        //     // new Question("日本語: 日本の首都はどこですか？", "とうきょう", new System.Globalization.CultureInfo("ja-JP")),
        // };
        //
        // foreach (var question in questions)
        // {
        //     Ask(question);
        // }
    }

    static void Ask(QuestionAnswer questionAnswer)
    {
        var response = AnsiConsole.Ask<string>($"{questionAnswer.Question} ");
        var isCorrect = questionAnswer.IsCorrectAnswer(response);
        var icon = isCorrect ? "[green]✔[/]" : "[red]✗[/]";

        AnsiConsole.Write("\e[1A\e[2K"); // Clear the previous line
        AnsiConsole.MarkupLine($"{questionAnswer.Question.EscapeMarkup()} {response.EscapeMarkup()} {icon}");
    }
}

internal class ListCommand : Command<ListCommand.Settings>
{
    public class Settings : CommandSettings;

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        // Store question data in <home folder>/.learn/
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".learn");
        Directory.CreateDirectory(dir);

        // Display files using Tree widget with nested structure
        var tree = new Tree(new Text(".learn"));
        PopulateTree(dir, tree);
        AnsiConsole.Write(tree);
        return 0;
    }

    // Recursively populate the tree with .csv files and subdirectories
    private static void PopulateTree(string directoryPath, IHasTreeNodes parentNode)
    {
        try
        {
            var csvFiles = Directory.GetFiles(directoryPath, "*.csv");
            Array.Sort(csvFiles, static (left, right) =>
                StringComparer.OrdinalIgnoreCase.Compare(Path.GetFileName(left), Path.GetFileName(right)));

            var subdirectories = Directory.GetDirectories(directoryPath);
            Array.Sort(subdirectories, static (left, right) =>
                StringComparer.OrdinalIgnoreCase.Compare(Path.GetFileName(left), Path.GetFileName(right)));

            if (csvFiles.Length > 0) // Add CSV files to the tree
            {
                foreach (var file in csvFiles)
                {
                    var fileName = Path.GetFileName(file);
                    parentNode.AddNode($"[green]{fileName.EscapeMarkup()}[/]");
                }
            }

            if (subdirectories.Length > 0) // Add subdirectories to the tree
            {
                foreach (var subdir in subdirectories)
                {
                    var folderName = Path.GetFileName(subdir);
                    var folderNode = parentNode.AddNode($"[blue]{folderName.EscapeMarkup()}[/]");
                    PopulateTree(subdir, folderNode);
                }
            }

            if (csvFiles.Length == 0 && subdirectories.Length == 0) // No files or subdirectories
            {
                parentNode.AddNode("[dim]no .csv files[/]");
            }
        }
        catch (Exception ex) // One bad folder does not break the whole tree
        {
            parentNode.AddNode($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
        }
    }
}