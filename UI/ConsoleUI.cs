using Spectre.Console;
using Color = Spectre.Console.Color;

namespace EnviroCLI.UI
{
    public static class ConsoleUI
    {
        public static void ShowTitle()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("EnviroCLI").Color(Color.Blue));
            AnsiConsole.WriteLine();

            var version = new Rule("[blue dim]v0.4.3[/]") { Style = Style.Parse("blue dim") };
            version.RightJustified();
            AnsiConsole.Write(version);
            AnsiConsole.WriteLine();
        }

        public static void ShowWelcomeMessage()
        {
            var panel = new Spectre.Console.Panel(
                new Markup(
                    "[white]Welcome to EnviroCLI![/]\n\n"
                        + "This tool helps you organize and launch multiple applications together.\n\n"
                        + "[blue]Quick Guide:[/]\n"
                        + "1. Create an environment (a group of apps)\n"
                        + "2. Add applications to your environment\n"
                        + "3. Initialize the environment to launch all apps\n\n"
                        + "Select [blue]Show Environments[/] to get started!"
                )
            )
            {
                Border = BoxBorder.Rounded,
                Padding = new Spectre.Console.Padding(2, 1, 2, 1),
            };

            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        public static string ShowMainMenu(string? lastUsedEnv)
        {
            ShowWelcomeMessage();
            var prompt = new SelectionPrompt<string>()
                .Title("[blue]Select an option[/]")
                .PageSize(10)
                .HighlightStyle(new Style(foreground: Color.Blue))
                .AddChoices(
                    new[]
                    {
                    lastUsedEnv is not null
                        ? $"Init Last Environment ({lastUsedEnv})"
                        : "Init Last Environment",
                    "Show Environments",
                    "Exit",
                    }
                );

            return AnsiConsole.Prompt(prompt);
        }
    }
}
