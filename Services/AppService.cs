using EnviroCLI.Models;
using Spectre.Console;
using static EnviroCLI.Services.ConfigurationService;
using static EnviroCLI.UI.ConsoleUI;
using static EnviroCLI.Utils.FindAppHelper;
using Color = Spectre.Console.Color;
using Environment = EnviroCLI.Models.Environment;

namespace EnviroCLI.Services
{
    public class AppService
    {
        public static void ManageApps(string configPath, Environment selectedEnv)
        {
            while (true)
            {
                Config jsonData = LoadConfig(configPath);
                var env = jsonData.Environment?.FirstOrDefault(e =>
                    e is not null
                    && e.Name is not null
                    && e.Name.Equals(selectedEnv.Name, StringComparison.OrdinalIgnoreCase)
                );

                if (env is null)
                {
                    AnsiConsole.MarkupLine("[red]Environment not found.[/]");
                    return;
                }

                AnsiConsole.Clear();
                ShowTitle();

                if (env.Apps.Count != 0)
                {
                    var table = new Table().RoundedBorder();
                    table.AddColumn("Order");
                    table.AddColumn("App");
                    table.AddColumn("Path");

                    foreach (var app in env.Apps.OrderBy(a => a.LaunchOrder))
                    {
                        if (app is not null)
                        {
                            table.AddRow(
                                new Markup($"[blue]{app.LaunchOrder}[/]"),
                                new Markup($"[white]{app.Name}[/]"),
                                new Markup($"[grey]{app.Route}[/]")
                            );
                        }
                    }

                    AnsiConsole.Write(table);
                    AnsiConsole.WriteLine();
                }

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Environment Apps[/]")
                        .PageSize(10)
                        .HighlightStyle(new Style(foreground: Color.Blue))
                        .AddChoices(["Add App", "Edit App", "Delete App", "Back"])
                );

                switch (choice)
                {
                    case "Add App":
                        AddApp(configPath, env);
                        break;
                    case "Edit App":
                        EditApp(configPath, env);
                        break;
                    case "Delete App":
                        DeleteApp(configPath, env);
                        break;
                    case "Back":
                        return;
                }
            }
        }

        public static void AddApp(string configPath, Environment selectedEnv)
        {
            var appName = AnsiConsole.Ask<string>("[white]App name[grey](Type 0 to go back)[/]:[/]");
            if (appName == "0")
                return;

            var commonApps = FindCommonApps();
            var matchingApps = commonApps
                .Where(a => a.Name.Contains(appName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            string? appPath = null;

            if (matchingApps.Count != 0)
            {
                var choices = new List<string>();
                choices.Add("Enter path manually");
                choices.Add("Back");
                choices.AddRange(matchingApps.Select(a => $"{a.Name} ({a.Path})"));

                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Select application path:[/]")
                        .PageSize(15)
                        .HighlightStyle(new Style(foreground: Color.Blue))
                        .AddChoices(choices)
                );

                if (selection == "Back")
                {
                    return;
                }

                if (selection != "Enter path manually")
                {
                    appPath = matchingApps.First(a => selection.StartsWith($"{a.Name} (")).Path;
                }
            }

            if (appPath == null)
            {
                AnsiConsole.MarkupLine(
                    $"We couldn't find the path for {appName}, please enter it manually"
                );
                appPath = AnsiConsole.Ask<string>("[white]App path (press 0 to go back):[/]");
                if (appPath == "0")
                {
                    return;
                }
            }


            if (appPath != "0")
            {
                if (!File.Exists(appPath))
                {
                    AnsiConsole.MarkupLine(
                        $"[red]The file '{appPath}' does not exist. Please provide a valid path.[/]"
                    );
                    appPath = AnsiConsole.Ask<string>("[white]App path (press 0 to go back):[/]");
                    return;
                }
            }

            var maxOrder =
                selectedEnv.Apps.Count != 0 ? selectedEnv.Apps.Max(a => a.LaunchOrder) : 0;
            var launchOrder = AnsiConsole.Ask<int>(
                $"[white]Launch order (current max: {maxOrder}):[/]",
                maxOrder + 1
            );

            Config config = LoadConfig(configPath);
            var env = config.Environment?.FirstOrDefault(e =>
                e is not null
                && e.Name is not null
                && e.Name.Equals(selectedEnv.Name, StringComparison.OrdinalIgnoreCase)
            );

            if (env is not null)
            {
                if (env.Apps is null)
                {
                    env.Apps = new List<App>();
                }

                var newApp = new App
                {
                    Name = appName,
                    Route = appPath,
                    LaunchOrder = launchOrder,
                };

                env.Apps.Add(newApp);
                SaveConfig(configPath, config);

                if (selectedEnv.Apps is null)
                {
                    selectedEnv.Apps = new List<App>();
                }
                selectedEnv.Apps.Add(newApp);

                AnsiConsole.MarkupLine($"[green]Added {appName} successfully![/]");
                Thread.Sleep(1000);
            }
        }

        public static void EditApp(string configPath, Environment selectedEnv)
        {
            if (selectedEnv.Apps is null || selectedEnv.Apps.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No apps to edit.[/]");
                return;
            }

            var appChoices = selectedEnv
                .Apps.Where(a => a is not null && !string.IsNullOrEmpty(a.Name))
                .Select(a => a.Name!)
                .ToList();

            if (appChoices.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No valid apps to edit.[/]");
                return;
            }

            appChoices.Add("Cancel");

            var appToEdit = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Select app to edit[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Blue))
                    .AddChoices(appChoices)
                    .EnableSearch()
            );

            if (appToEdit == "Cancel")
                return;

            var app = selectedEnv.Apps.FirstOrDefault(a => a.Name == appToEdit);
            if (app is null)
                return;

            var newName = AnsiConsole.Ask("[white]New app name:[/]", app.Name ?? "");
            var newPath = AnsiConsole.Ask("[white]New app path:[/]", app.Route ?? "");

            if (!File.Exists(newPath))
            {
                AnsiConsole.MarkupLine(
                    $"[red]The file '{newPath}' does not exist. Please provide a valid path.[/]"
                );
                return;
            }

            var maxOrder = selectedEnv.Apps.Max(a => a.LaunchOrder);
            var newOrder = AnsiConsole.Ask("[white]New launch order:[/]", app.LaunchOrder);

            Config config = LoadConfig(configPath);
            var env = config.Environment?.FirstOrDefault(e =>
                e is not null
                && e.Name is not null
                && e.Name.Equals(selectedEnv.Name, StringComparison.OrdinalIgnoreCase)
            );

            if (env?.Apps is not null)
            {
                var targetApp = env.Apps.FirstOrDefault(a => a.Name == appToEdit);
                if (targetApp is not null)
                {
                    targetApp.Name = newName;
                    targetApp.Route = newPath;
                    targetApp.LaunchOrder = newOrder;
                    SaveConfig(configPath, config);

                    app.Name = newName;
                    app.Route = newPath;
                    app.LaunchOrder = newOrder;
                }
            }
        }

        public static void DeleteApp(string configPath, Environment selectedEnv)
        {
            if (selectedEnv.Apps is null || selectedEnv.Apps.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No apps to delete.[/]");
                return;
            }

            var appChoices = selectedEnv
                .Apps.Where(a => a is not null && !string.IsNullOrEmpty(a.Name))
                .Select(a => a.Name!)
                .ToList();

            if (appChoices.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No valid apps to delete.[/]");
                return;
            }

            appChoices.Add("Cancel");

            var appToDelete = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Select app to delete[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Blue))
                    .AddChoices(appChoices)
                    .EnableSearch()
            );

            if (appToDelete == "Cancel")
                return;

            Config config = LoadConfig(configPath);
            var env = config.Environment?.FirstOrDefault(e =>
                e is not null
                && e.Name is not null
                && e.Name.Equals(selectedEnv.Name, StringComparison.OrdinalIgnoreCase)
            );

            if (env?.Apps is not null)
            {
                env.Apps.RemoveAll(a => a is not null && a.Name == appToDelete);
                SaveConfig(configPath, config);
                selectedEnv.Apps = env.Apps;
            }
        }
    }
}
