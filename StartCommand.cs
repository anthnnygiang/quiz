using System.ComponentModel;
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
        // read quiz files form .quiz dir and start quiz
        Console.WriteLine(settings.QuizFile);
        const string quizDirectory = ".quiz";
        var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), quizDirectory, settings.QuizFile);
        if (!IsValidQuizFile(filePath))
        {
            Console.WriteLine($"Invalid quiz file: {settings.QuizFile}");
            return 1;
        }

        foreach (var line in File.ReadLines(filePath))
        {
            Console.WriteLine(line);
        }

        return 0;
    }

    private static bool IsValidQuizFile(string filePath)
    {
        // check tsv format
        const int expectedCols = 3;
        if (!File.Exists(filePath))
        {
            return false;
        }

        if (!Path.GetExtension(filePath).Equals(".tsv", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (File.ReadLines(filePath).Select(line => line.Split('\t')).Any(cols => cols.Length != expectedCols))
        {
            return false;
        }

        return true;
    }
}