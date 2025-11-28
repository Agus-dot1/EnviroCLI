using EnviroCLI.Models;
using Spectre.Console;
using System.Diagnostics;
using System.Text.Json;
using static EnviroCLI.Services.AppService;
using static EnviroCLI.Services.ConfigurationService;
using static EnviroCLI.UI.ConsoleUI;
using static EnviroCLI.Utils.FindAppHelper;
using Color = Spectre.Console.Color;
using Environment = EnviroCLI.Models.Environment;

namespace EnviroCLI.Services
{
    public class EnvironmentService
    {
        public static void InitializeEnvironment(string configPath, string envName)
        {
            Config jsonData = LoadConfig(configPath);
            var selectedEnv = jsonData.Environment?.FirstOrDefault(e =>
                e?.Name is not null && e.Name.Equals(envName, StringComparison.OrdinalIgnoreCase)
            );

            if (selectedEnv is null)
            {
                AnsiConsole.MarkupLine("[red]Environment not found.[/]");
                Thread.Sleep(1500);
                return;
            }
            if (selectedEnv.Apps.Count is 0)
            {
                AnsiConsole.MarkupLine("[red]No apps found in the selected environment.[/]");
                Thread.Sleep(1500);
                return;
            }

            jsonData.LastUsedEnvironment = selectedEnv.Name;
            SaveConfig(configPath, jsonData);

            var table = new Table().RoundedBorder();
            table.AddColumn("App");
            table.AddColumn("Status");

            AnsiConsole
                .Live(table)
                .AutoClear(false)
                .Start(ctx =>
                {
                    var appsToLaunch =
                        selectedEnv.Apps?.OrderBy(a => a.LaunchOrder).ToList() ?? new List<App>();

                    foreach (var app in appsToLaunch)
                    {
                        if (app is null || string.IsNullOrEmpty(app.Route))
                        {
                            continue;
                        }

                        table.AddRow(
                            new Markup($"[blue]{app.Name}[/]"),
                            new Markup("[yellow]Starting...[/]")
                        );
                        ctx.Refresh();

                        if (!File.Exists(app.Route))
                        {
                            table.UpdateCell(
                                table.Rows.Count - 1,
                                1,
                                new Markup($"[red]Error: File not found[/]")
                            );
                            ctx.Refresh();
                            continue;
                        }

                        try
                        {
                            // the only way that the app will start without the bloat of the shell
                            ProcessStartInfo startInfo = new ProcessStartInfo
                            {
                                FileName = app.Route,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                            };

                            var process = Process.Start(startInfo);
                            table.UpdateCell(table.Rows.Count - 1, 1, new Markup("[green]Started[/]"));
                            ctx.Refresh();
                        }
                        catch (Exception ex)
                        {
                            table.UpdateCell(
                                table.Rows.Count - 1,
                                1,
                                new Markup($"[red]Error: {ex.Message}[/]")
                            );
                            ctx.Refresh();
                        }
                    }
                });

            AnsiConsole.MarkupLine("\n[blue]Press any key to continue...[/]");
            Console.ReadKey(true);
            AnsiConsole.Clear();
        }

        public static void ManageEnvironments(string configPath, ref string? lastUsedEnv)
        {
            while (true)
            {
                AnsiConsole.Clear();
                ShowTitle();

                string jsonContent = File.ReadAllText(configPath);
                Config? configData = JsonSerializer.Deserialize<Config>(jsonContent);
                List<Environment> environments = configData?.Environment ?? new List<Environment>();

                if (environments.Count == 0)
                {
                    AnsiConsole.MarkupLine("\n[blue]No environments found. Please add an environment.[/]");
                }
                else
                {
                    EnvironmentList(configPath);
                }

                var option = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Select an option[/]")
                        .PageSize(10)
                        .HighlightStyle(new Style(foreground: Color.Blue))
                        .UseConverter(x => x)
                        .AddChoices(
                            [
                            "Initialize Environment",
                            "Add Environment",
                            "Edit Environment",
                            "Delete Environment",
                            "Back",
                            ]
                        )
                );

                switch (option)
                {
                    case "Initialize Environment":
                        if (environments.Count == 0)
                        {
                            AnsiConsole.MarkupLine("[red]No environments found.[/]");
                            break;
                        }

                        var envNames = environments
                            .Where(e => e?.Name is not null)
                            .Select(e => e!.Name!)
                            .ToList();
                        envNames.Add("Back");

                        var selectedEnv = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("[blue]Select environment to initialize[/]")
                                .PageSize(10)
                                .HighlightStyle(new Style(foreground: Color.Blue))
                                .UseConverter(x => x)
                                .AddChoices(envNames)
                                .EnableSearch()
                        );

                        if (selectedEnv == "Back")
                        {
                            break;
                        }

                        InitializeEnvironment(configPath, selectedEnv);
                        lastUsedEnv = selectedEnv;
                        break;
                    case "Add Environment":
                        AddEnvironment(configPath);
                        break;
                    case "Edit Environment":
                        EditEnvironment(configPath);
                        break;
                    case "Delete Environment":
                        DeleteEnvironment(configPath);
                        break;
                    case "Back":
                        return;
                }
            }
        }

        public static void EnvironmentList(string configPath)
        {
            if (!File.Exists(configPath))
            {
                AnsiConsole.MarkupLine("[red]The config file doesn't exist[/]");
                return;
            }
            string jsonContent = File.ReadAllText(configPath);
            Config? configData = JsonSerializer.Deserialize<Config>(jsonContent);
            List<Environment> environments = configData?.Environment ?? new List<Environment>();

            if (environments is null)
            {
                return;
            }

            var table = new Table().RoundedBorder();
            table.AddColumn("Environment");
            table.AddColumn("Apps (Launch Order)");


            foreach (var env in environments)
            {
                if (env is not null)
                {
                    var orderedApps = env.Apps?.OrderBy(a => a.LaunchOrder).ToList() ?? new List<App>();
                    var appsList = string.Join(
                        "\n",
                        orderedApps.Select((a, i) => $"{i + 1}. [blue]{a.Name}[/] ({a.LaunchOrder})")
                    );
                    table.AddRow(new Markup($"[blue]{env.Name ?? ""}[/]"), new Markup($"{appsList}"));
                }
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }

        public static void AddEnvironment(string configPath)
        {
            AnsiConsole.Clear();
            ShowTitle();
            var envName = AnsiConsole.Ask<string>("[white]Name your environment:[/]");
            var newEnv = new Environment { Name = envName, Apps = new List<App>() };

            AnsiConsole.MarkupLine("\n[blue]Add apps to the environment[/]");

            int appOrder = 1;
            bool addingApps = true;
            while (addingApps)
            {
                var action = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]What would you like to do?[/]")
                        .PageSize(10)
                        .HighlightStyle(new Style(foreground: Color.Blue))
                        .AddChoices(["Add App", "Finish"])
                );

                if (action == "Finish")
                {
                    addingApps = false;
                    break;
                }

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
                    var choices = matchingApps.Select(a => $"{a.Name} ({a.Path})").ToList();
                    choices.Insert(0, "manual");

                    var selection = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title(
                                "[blue]Select application path (if the app is not on the list, select 'manual' for custom path):[/]"
                            )
                            .PageSize(15)
                            .HighlightStyle(new Style(foreground: Color.Blue))
                            .AddChoices(choices)
                    );

                    if (selection == "manual")
                    {
                        appPath = null;
                    }
                    else
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
                    if (string.IsNullOrWhiteSpace(appPath))
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
                        continue;
                    }
                }

                var launchOrderPrompt = AnsiConsole.Prompt(
                    new TextPrompt<string>(
                        "[white]Launch order (press Enter for {0}, 'b' to go back):[/]"
                    )
                        .DefaultValue(appOrder.ToString())
                        .ValidationErrorMessage("[red]Please enter a valid number or 'b' to go back[/]")
                        .Validate(input =>
                        {
                            if (input.Trim().Equals("b", StringComparison.CurrentCultureIgnoreCase))
                                return ValidationResult.Success();
                            if (int.TryParse(input, out int value))
                                return ValidationResult.Success();
                            return ValidationResult.Error();
                        })
                );

                if (launchOrderPrompt.Trim().Equals("b", StringComparison.CurrentCultureIgnoreCase))
                {
                    continue;
                }

                var launchOrder = int.Parse(
                    string.IsNullOrWhiteSpace(launchOrderPrompt)
                        ? appOrder.ToString()
                        : launchOrderPrompt
                );

                var newApp = new App
                {
                    Name = appName,
                    Route = appPath,
                    LaunchOrder = launchOrder,
                };

                newEnv.Apps.Add(newApp);
                AnsiConsole.MarkupLine($"[green]Added app: {appName}[/]");
                appOrder++;
            }

            try
            {
                var configData = LoadConfig(configPath);

                if (configData.Environment == null)
                {
                    configData.Environment = new List<Environment>();
                }

                configData.Environment.Add(newEnv);
                SaveConfig(configPath, configData);

                AnsiConsole.MarkupLine(
                    $"[green]Environment '{envName}' added with {newEnv.Apps.Count} apps![/]"
                );
                Thread.Sleep(1500);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error saving environment: {ex.Message}[/]");
                AnsiConsole.MarkupLine("[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }

        public static void EditEnvironment(string configPath)
        {
            AnsiConsole.Clear();
            ShowTitle();
            EnvironmentList(configPath);

            Config jsonData = LoadConfig(configPath);
            List<Environment> environments = jsonData.Environment ?? new List<Environment>();

            if (environments.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No environments found.[/]");
                Thread.Sleep(1500);
                return;
            }

            var envNames = environments.Where(e => e?.Name is not null).Select(e => e!.Name!).ToList();
            envNames.Add("Back");

            var enviro = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Select environment to edit[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Blue))
                    .UseConverter(x => x)
                    .AddChoices(envNames)
                    .EnableSearch()
            );

            if (enviro == "Back")
            {
                return;
            }

            var selectedEnv = environments.Find(e =>
                e is not null
                && e.Name is not null
                && e.Name.Equals(enviro, StringComparison.OrdinalIgnoreCase)
            );

            if (selectedEnv is null)
            {
                AnsiConsole.MarkupLine("[red]Environment not found.[/]");
                Thread.Sleep(1500);
                return;
            }

            AnsiConsole.MarkupLine($"[Blue]{selectedEnv.Name}[/]\n");

            if (selectedEnv.Apps is not null)
            {
                foreach (var app in selectedEnv.Apps.OrderBy(a => a.LaunchOrder))
                {
                    AnsiConsole.MarkupLine($"{app.Name} - [grey]{app.Route}[/]");
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[red]No apps found in the selected environment.[/]");
                Thread.Sleep(1500);
            }

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]What would you like to edit?[/]")
                    .UseConverter(x => x)
                    .AddChoices("Environment Name", "Environment Apps", "Back")
            );

            switch (option)
            {
                case "Environment Name":
                    string newName = AnsiConsole.Ask<string>("Enter the new environment name: ");
                    if (!string.IsNullOrWhiteSpace(newName))
                    {
                        selectedEnv.Name = newName;
                        SaveConfig(configPath, jsonData);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Invalid environment name.[/]");
                        Thread.Sleep(1500);
                    }
                    break;
                case "Environment Apps":
                    ManageApps(configPath, selectedEnv);
                    break;
                case "Back":
                    return;
            }
        }

        public static void DeleteEnvironment(string configPath)
        {
            AnsiConsole.Clear();
            ShowTitle();
            EnvironmentList(configPath);

            Config jsonData = LoadConfig(configPath);
            List<Environment> environments = jsonData.Environment ?? new List<Environment>();

            if (environments.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No environments found.[/]");
                Thread.Sleep(1500);
                return;
            }

            var envNames = environments.Where(e => e?.Name is not null).Select(e => e!.Name!).ToList();
            envNames.Add("Back");

            var enviro = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[red]Select environment to delete[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Red))
                    .UseConverter(x => x)
                    .AddChoices(envNames)
                    .EnableSearch()
            );

            if (enviro == "Back")
            {
                return;
            }

            var selectedEnv = environments.Find(e =>
                e is not null
                && e.Name is not null
                && e.Name.Equals(enviro, StringComparison.OrdinalIgnoreCase)
            );

            if (selectedEnv is null)
            {
                AnsiConsole.MarkupLine("[red]Selected environment not found.[/]");
                Thread.Sleep(1500);
                return;
            }

            if (
                AnsiConsole.Confirm(
                    $"[red]Are you sure you want to delete environment '{selectedEnv.Name}'?[/]"
                )
            )
            {
                environments.Remove(selectedEnv);

                if (jsonData.LastUsedEnvironment == selectedEnv.Name)
                {
                    jsonData.LastUsedEnvironment = null;
                }

                SaveConfig(configPath, jsonData);
                AnsiConsole.MarkupLine("[green]Environment deleted successfully![/]");
                Thread.Sleep(1500);
            }
        }
    }
}
