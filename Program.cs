using Spectre.Console;
using Spectre.Console.Cli;

namespace Quiz;

public class Program
{
    static void Main(string[] args)
    {
        var app = new CommandApp();
        app.Configure(config =>
        {
            // Info
            config.SetApplicationName("quiz");
            config.SetApplicationVersion("1.0.0");

            // Commands
            config.AddCommand<StartCommand>("start")
                .WithDescription("Start quiz")
                .WithAlias("st");
            config.AddCommand<ListCommand>("list")
                .WithDescription("List all quiz files")
                .WithAlias("ls");

            // Examples
            config.AddExample("list");
            config.AddExample("start questions.tsv");
            config.AddExample("start geography/questions.tsv");
        });
        app.Run(args);

        // var questions = new[]
        // {
        //     new QuestionAnswer("What is the capital of France?", "paris"),
        //     new QuestionAnswer("What is the largest planet in the solar system?", "Jupiter"),
        //     new QuestionAnswer("What is the chemical symbol for gold?", "Au"),
        //     new QuestionAnswer("What is the square root of 16?", "4"),
        //     // Future examples: (needs a dynamic dictionary of cultureInfo objects for supported languages)
        //     // new Question("Tiếng Việt: thủ đô của Việt Nam là gì?", "Hà Nội", new System.Globalization.CultureInfo("vi-VN")),
        //     // new Question("日本語: 日本の首都はどこですか？", "とうきょう", new System.Globalization.CultureInfo("ja-JP")),
        // };
        //
        // foreach (var question in questions)
        // {
        //     question.Ask();
        // }
    }
}
