using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Learn;

public class Program
{
    static void Main(string[] args)
    {
        // var name = AnsiConsole.Ask<string>("What's your [green]name[/]?");
        // AnsiConsole.MarkupLine($"Hello, [blue]{name}[/]!");

        Ask("What is the capital of France?", "paris");
        Ask("What is the largest planet in the solar system?", "jupiter");
        Ask("What is the chemical symbol for gold?", "au");
        Ask("What is the square root of 16?", "4");
        
        // AnsiConsole.MarkupLine("[green] ✔[/] Welcome to the Spectre.Console.Cli demo!");
        // AnsiConsole.MarkupLineInterpolated($"[red bold] ✗ Error:[/] Missing dependency '{"error"}'");


        // var app = new CommandApp();
        // app.Configure(config => { config.AddCommand<GreetCommand>("greet"); });
        // app.Run(args);
    }
    static void Ask(string question, string correctAnswer)
    {
        AnsiConsole.Markup($"{question} ");
        var answer = Console.ReadLine() ?? "";

        // Move up one line and clear it
        AnsiConsole.Write("\e[1A\e[2K");

        var icon = answer.Trim().Equals(correctAnswer, StringComparison.OrdinalIgnoreCase) ? "[green]✔[/]" : "[red]✗[/]";
        AnsiConsole.MarkupLine($"{question} {answer} {icon}");
    }
}

internal class GreetCommand : Command<GreetCommand.Settings>
{
    public class Settings : CommandSettings
    {
        [CommandArgument(0, "<name>")]
        [Description("The name to greet")]
        public string Name { get; init; } = string.Empty;
    }

    protected override int Execute(CommandContext context, Settings settings, CancellationToken cancellation)
    {
        System.Console.WriteLine($"Hello, {settings.Name}!");
        return 0;
    }
}