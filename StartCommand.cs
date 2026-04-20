using System.ComponentModel;
using System.Globalization;
using Spectre.Console.Cli;

namespace Quiz;

public class StartCommand : Command<StartCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<file.tsv>")]
        [Description("The quiz file")]
        public required string QuizFile { get; init; }
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        // read quiz files form .quiz directory and start quiz
        Console.WriteLine(settings.QuizFile);
        const string quizDirectory = ".quiz";
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), quizDirectory,
            settings.QuizFile);
        if (!IsValidQuizFile(filePath))
        {
            return 1;
        }

        foreach (var line in File.ReadLines(filePath))
        {
            Console.WriteLine(line);
            var cols = line.Split('\t');
            var card = new Card(cols[0], cols[1], cols[2]);
            card.Ask();
        }

        return 0;
    }

    private static bool IsValidQuizFile(string filePath)
    {
        // check tsv format
        const int expectedCols = 3;
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            return false;
        }

        if (!Path.GetExtension(filePath).Equals(".tsv", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine($"Invalid file extension: {filePath}. Expected .tsv");
            return false;
        }

        if (File.ReadLines(filePath).Select(line => line.Split('\t')).Any(cols => cols.Length != expectedCols))
        {
            Console.WriteLine($"Expected {expectedCols} columns separated by tabs");
            return false;
        }

        return true;
    }
}