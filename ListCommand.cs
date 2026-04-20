using Spectre.Console;
using Spectre.Console.Cli;

namespace Quiz;

internal class ListCommand : Command<ListCommand.Settings>
{
    public class Settings : CommandSettings;

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        // Store question data in <home folder>/.quiz/
        const string quizDirectory = ".quiz";
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"{quizDirectory}");
        Directory.CreateDirectory(dir);

        // Display files using Tree widget with nested structure
        var tree = new Tree($"[blue]{quizDirectory}[/]");
        PopulateTree(dir, tree);
        AnsiConsole.Write(tree);
        return 0;
    }

    // Recursively populate the tree with .tsv files and subdirectories
    private static void PopulateTree(string directoryPath, IHasTreeNodes parentNode)
    {
        try
        {
            var tsvFiles = Directory.GetFiles(directoryPath, "*.tsv");
            Array.Sort(tsvFiles, static (left, right) =>
                StringComparer.OrdinalIgnoreCase.Compare(Path.GetFileName(left), Path.GetFileName(right)));

            var subdirectories = Directory.GetDirectories(directoryPath);
            Array.Sort(subdirectories, static (left, right) =>
                StringComparer.OrdinalIgnoreCase.Compare(Path.GetFileName(left), Path.GetFileName(right)));

            if (tsvFiles.Length > 0) // Add tsv files to the tree
            {
                foreach (var file in tsvFiles)
                {
                    var fileName = Path.GetFileName(file);
                    parentNode.AddNode($"{fileName.EscapeMarkup()}");
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

            if (tsvFiles.Length == 0 && subdirectories.Length == 0) // No files or subdirectories
            {
                parentNode.AddNode("[dim]empty[/]");
            }
        }
        catch (Exception ex) // One bad folder does not break the whole tree
        {
            parentNode.AddNode($"[red]Error: {ex.Message.EscapeMarkup()}[/]");
        }
    }
}
