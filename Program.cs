using EnviroCLI.Models;
using EnviroCLI.Services;
using Spectre.Console;
using static EnviroCLI.Services.EnvironmentService;
using static EnviroCLI.UI.ConsoleUI;


namespace EnviroCLI;

class Program : ConfigurationService
{
    static void Main()
    {
        string configPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "config",
            "config.json"
        );

        var configDir = Path.GetDirectoryName(configPath);
        if (!Directory.Exists(configDir) && configDir != null)
        {
            Directory.CreateDirectory(configDir);
        }

        Config config = LoadConfig(configPath);
        string? lastUsedEnv = config.LastUsedEnvironment;

        while (true)
        {
            AnsiConsole.Clear();
            ShowTitle();
            string option = ShowMainMenu(lastUsedEnv);

            switch (option)
            {
                case var o when o.StartsWith("Init Last Environment"):
                    if (lastUsedEnv is not null)
                    {
                        InitializeEnvironment(configPath, lastUsedEnv);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[yellow]No environment has been used yet.[/]");
                        AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                        Console.ReadKey();
                    }
                    break;
                case "Show Environments":
                    ManageEnvironments(configPath, ref lastUsedEnv);
                    break;
                case "Exit":
                    AnsiConsole.MarkupLine("[green]Thanks for using EnviroCLI![/]");
                    return;
            }
        }
    }
}