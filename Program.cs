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
            config.AddExample("start questions.json");
            config.AddExample("start geography/questions.json");
        });
        app.Run(args);
    }
}
