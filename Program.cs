using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Spectre.Console;
using Color = Spectre.Console.Color;

namespace EnviroCLI;

#region Models
public class App
{
    public string? Name { get; set; } = string.Empty;
    public string? Route { get; set; } = string.Empty;
    public int LaunchOrder { get; set; } = 0;
}

public class Environment
{
    public string? Name { get; set; } = string.Empty;
    public List<App>? Apps { get; set; } = new List<App>();
}

public class Config
{
    public List<Environment>? Environment { get; set; } = new List<Environment>();
    public string? LastUsedEnvironment { get; set; } = string.Empty;
}
#endregion

class Program
{
    #region Configuration Management
    private static Config LoadConfig(string configPath)
    {
        try
        {
            // Ensure config directory exists
            var configDir = Path.GetDirectoryName(configPath);
            if (!Directory.Exists(configDir) && configDir != null)
            {
                Directory.CreateDirectory(configDir);
                AnsiConsole.MarkupLine("[blue]Created config directory[/]");
            }

            if (File.Exists(configPath))
            {
                var jsonString = File.ReadAllText(configPath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true,
                };

                var config = JsonSerializer.Deserialize<Config>(jsonString, options);
                if (config?.Environment == null)
                {
                    config = new Config { Environment = new List<Environment>() };
                }
                return config;
            }
            else
            {
                // Create new config file with empty structure
                var newConfig = new Config { Environment = new List<Environment>(), LastUsedEnvironment = null };
                SaveConfig(configPath, newConfig);
                AnsiConsole.MarkupLine("[blue]Created new config file[/]");
                return newConfig;
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error loading config: {ex.Message}[/]");
            return new Config { Environment = new List<Environment>(), LastUsedEnvironment = null };
        }
    }

    private static void SaveConfig(string configPath, Config config)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true,
            };

            var jsonString = JsonSerializer.Serialize(config, options);
            File.WriteAllText(configPath, jsonString);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error saving config: {ex.Message}[/]");
        }
    }
    #endregion

    #region UI Components
    private static void ShowTitle()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("EnviroCLI").Color(Color.Blue));
        AnsiConsole.WriteLine();

        var version = new Rule("[blue dim]v0.4.3[/]") { Style = Style.Parse("blue dim") };
        version.RightJustified();
        AnsiConsole.Write(version);
        AnsiConsole.WriteLine();
    }

    private static void ShowWelcomeMessage()
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

    private static string ShowMainMenu(string? lastUsedEnv)
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
    #endregion

    #region Environment Management
    private static void InitializeEnvironment(string configPath, string envName)
    {
        Config jsonData = LoadConfig(configPath);
        var selectedEnv = jsonData.Environment?.FirstOrDefault(e =>
            e?.Name is not null && e.Name.Equals(envName, StringComparison.OrdinalIgnoreCase)
        );

        if (selectedEnv is null)
        {
            AnsiConsole.MarkupLine("[red]Environment not found.[/]");
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

    private static void ManageEnvironments(string configPath, ref string? lastUsedEnv)
    {
        while (true)
        {
            AnsiConsole.Clear();
            ShowTitle();
            EnvironmentList(configPath);

            var option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Select an option[/]")
                    .PageSize(10)
                    .HighlightStyle(new Style(foreground: Color.Blue))
                    .UseConverter(x => x)
                    .AddChoices(
                        new[]
                        {
                            "Initialize Environment",
                            "Add Environment",
                            "Edit Environment",
                            "Delete Environment",
                            "Back",
                        }
                    )
            );

            switch (option)
            {
                case "Initialize Environment":
                    var config = LoadConfig(configPath);
                    var environments = config.Environment ?? new List<Environment>();

                    if (!environments.Any())
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

    private static void EnvironmentList(string configPath)
    {
        if (!File.Exists(configPath))
        {
            AnsiConsole.MarkupLine("[red]The config file doesn't exist[/]");
            return;
        }

        var table = new Table().RoundedBorder();
        table.AddColumn("Environment");
        table.AddColumn("Apps (Launch Order)");
        string jsonContent = File.ReadAllText(configPath);
        Config? configData = JsonSerializer.Deserialize<Config>(jsonContent);
        List<Environment> environments = configData?.Environment ?? new List<Environment>();

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

    private static void AddEnvironment(string configPath)
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
                    .AddChoices(new[] { "Add App", "Finish" })
            );

            if (action == "Finish")
            {
                addingApps = false;
                break;
            }

            var appName = AnsiConsole.Ask<string>("[white]App name:[/]");
            if (string.IsNullOrWhiteSpace(appName))
            {
                continue;
            }

            // Try to find matching apps
            var commonApps = FindCommonApps();
            var matchingApps = commonApps
                .Where(a => a.Name.Contains(appName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            string? appPath = null;

            if (matchingApps.Any())
            {
                var choices = matchingApps.Select(a => $"{a.Name} ({a.Path})").ToList();
                choices.Insert(0, "manual");

                var selection = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title(
                            "[blue]Select application path (or type 'manual' for custom path):[/]"
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
                appPath = AnsiConsole.Ask<string>("[white]App path (press Enter to go back):[/]");
                if (string.IsNullOrWhiteSpace(appPath))
                {
                    continue;
                }
            }

            if (!File.Exists(appPath))
            {
                AnsiConsole.MarkupLine(
                    $"[red]The file '{appPath}' does not exist. Please provide a valid path.[/]"
                );
                continue;
            }

            var launchOrderPrompt = AnsiConsole.Prompt(
                new TextPrompt<string>(
                    "[white]Launch order (press Enter for {0}, 'b' to go back):[/]"
                )
                    .DefaultValue(appOrder.ToString())
                    .ValidationErrorMessage("[red]Please enter a valid number or 'b' to go back[/]")
                    .Validate(input =>
                    {
                        if (input.Trim().ToLower() == "b")
                            return ValidationResult.Success();
                        if (int.TryParse(input, out int value))
                            return ValidationResult.Success();
                        return ValidationResult.Error();
                    })
            );

            if (launchOrderPrompt.Trim().ToLower() == "b")
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

    private static void EditEnvironment(string configPath)
    {
        AnsiConsole.Clear();
        ShowTitle();
        EnvironmentList(configPath);

        Config jsonData = LoadConfig(configPath);
        List<Environment> environments = jsonData.Environment ?? new List<Environment>();

        if (!environments.Any())
        {
            AnsiConsole.MarkupLine("[red]No environments found.[/]");
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
                }
                break;
            case "Environment Apps":
                ManageApps(configPath, selectedEnv);
                break;
            case "Back":
                return;
        }
    }

    private static void DeleteEnvironment(string configPath)
    {
        AnsiConsole.Clear();
        ShowTitle();
        EnvironmentList(configPath);

        Config jsonData = LoadConfig(configPath);
        List<Environment> environments = jsonData.Environment ?? new List<Environment>();

        if (!environments.Any())
        {
            AnsiConsole.MarkupLine("[red]No environments found.[/]");
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
            return;
        }

        if (
            AnsiConsole.Confirm(
                $"[red]Are you sure you want to delete environment '{selectedEnv.Name}'?[/]"
            )
        )
        {
            environments.Remove(selectedEnv);

            // If we're deleting the last used environment, clear it
            if (jsonData.LastUsedEnvironment == selectedEnv.Name)
            {
                jsonData.LastUsedEnvironment = null;
            }

            SaveConfig(configPath, jsonData);
            AnsiConsole.MarkupLine("[green]Environment deleted successfully![/]");
        }
    }
    #endregion

    #region App Management
    private static void ManageApps(string configPath, Environment selectedEnv)
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

            // Display apps table
            if (env.Apps?.Any() == true)
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
                    .AddChoices(new[] { "Add App", "Edit App", "Delete App", "Back" })
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

    private static void AddApp(string configPath, Environment selectedEnv)
    {
        var appName = AnsiConsole.Ask<string>("[white]App name[grey](Type 0 to go back)[/]:[/]");
        if (appName == "0")
            return;

        // Try to find matching apps
        var commonApps = FindCommonApps();
        var matchingApps = commonApps
            .Where(a => a.Name.Contains(appName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        string? appPath = null;

        if (matchingApps.Any())
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
            appPath = AnsiConsole.Ask<string>("[white]App path:[/]");
        }
        if (!File.Exists(appPath))
        {
            AnsiConsole.MarkupLine(
                $"[red]The file '{appPath}' does not exist. Please provide a valid path.[/]"
            );
            return;
        }

        var maxOrder =
            selectedEnv.Apps?.Any() == true ? selectedEnv.Apps.Max(a => a.LaunchOrder) : 0;
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

            // Update the selectedEnv to reflect changes
            if (selectedEnv.Apps is null)
            {
                selectedEnv.Apps = new List<App>();
            }
            selectedEnv.Apps.Add(newApp);

            AnsiConsole.MarkupLine($"[green]Added {appName} successfully![/]");
            System.Threading.Thread.Sleep(1000);
        }
    }

    private static void EditApp(string configPath, Environment selectedEnv)
    {
        if (selectedEnv.Apps is null || !selectedEnv.Apps.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No apps to edit.[/]");
            return;
        }

        var appChoices = selectedEnv
            .Apps.Where(a => a is not null && !string.IsNullOrEmpty(a.Name))
            .Select(a => a.Name!)
            .ToList();

        if (!appChoices.Any())
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

                // Update the selectedEnv to reflect changes
                app.Name = newName;
                app.Route = newPath;
                app.LaunchOrder = newOrder;
            }
        }
    }

    private static void DeleteApp(string configPath, Environment selectedEnv)
    {
        if (selectedEnv.Apps is null || !selectedEnv.Apps.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No apps to delete.[/]");
            return;
        }

        var appChoices = selectedEnv
            .Apps.Where(a => a is not null && !string.IsNullOrEmpty(a.Name))
            .Select(a => a.Name!)
            .ToList();

        if (!appChoices.Any())
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
    #endregion

    #region App Detection and Validation
    private static List<(string Name, string Path)> FindCommonApps()
    {
        var apps = new HashSet<(string Name, string Path)>();

        // Common installation paths for applications
        var commonPaths = new[]
        {
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles),
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFilesX86),
            Path.Combine(
                System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.LocalApplicationData
                )
            ),
            Path.Combine(
                System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)
            ),
            // Add VSCode specific paths
            Path.Combine(
                System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.LocalApplicationData
                ),
                "Programs",
                "Microsoft VS Code"
            ),
            // Add common browser paths
            @"C:\Program Files\Google\Chrome\Application",
            @"C:\Program Files\Mozilla Firefox",
            @"C:\Program Files (x86)\Microsoft\Edge\Application",
            // Add common dev tool paths
            @"C:\Program Files\Git\cmd",
            @"C:\Program Files\Docker\Docker\resources",
            // Add common chat apps
            Path.Combine(
                System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.LocalApplicationData
                ),
                "Discord"
            ),
            Path.Combine(
                System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.LocalApplicationData
                ),
                "Slack"
            ),
            // Add gaming paths
            @"C:\Program Files (x86)\Steam",
            @"C:\Program Files\Epic Games\Launcher\Portal\Binaries\Win64",
        };

        AnsiConsole.MarkupLine("[grey]Searching in common paths:[/]");
        foreach (var basePath in commonPaths)
        {
            if (!Directory.Exists(basePath))
                continue;
            AnsiConsole.MarkupLine($"[grey]- {basePath}[/]");

            try
            {
                // First try direct .exe files in the directory
                foreach (
                    var exe in Directory.GetFiles(basePath, "*.exe", SearchOption.TopDirectoryOnly)
                )
                {
                    var fileName = Path.GetFileNameWithoutExtension(exe);
                    if (IsValidAppName(fileName))
                    {
                        apps.Add((fileName, exe));
                        AnsiConsole.MarkupLine($"[grey]Found: {fileName} at {exe}[/]");
                    }
                }

                // Then check immediate subdirectories (only one level deep)
                foreach (
                    var dir in Directory.GetDirectories(
                        basePath,
                        "*",
                        SearchOption.TopDirectoryOnly
                    )
                )
                {
                    try
                    {
                        foreach (
                            var exe in Directory.GetFiles(
                                dir,
                                "*.exe",
                                SearchOption.TopDirectoryOnly
                            )
                        )
                        {
                            var fileName = Path.GetFileNameWithoutExtension(exe);
                            if (IsValidAppName(fileName))
                            {
                                apps.Add((fileName, exe));
                                AnsiConsole.MarkupLine($"[grey]Found: {fileName} at {exe}[/]");
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        AnsiConsole.MarkupLine($"[grey]Access denied to {dir}[/]");
                    }
                    catch (IOException)
                    {
                        AnsiConsole.MarkupLine($"[grey]IO error in {dir}[/]");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                AnsiConsole.MarkupLine($"[grey]Access denied to {basePath}[/]");
            }
            catch (IOException)
            {
                AnsiConsole.MarkupLine($"[grey]IO error in {basePath}[/]");
            }
        }

        // Search in PATH last
        var pathDirs =
            System.Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator)
            ?? Array.Empty<string>();
        foreach (var dir in pathDirs.Where(Directory.Exists))
        {
            try
            {
                foreach (var exe in Directory.GetFiles(dir, "*.exe", SearchOption.TopDirectoryOnly))
                {
                    var fileName = Path.GetFileNameWithoutExtension(exe);
                    if (IsValidAppName(fileName))
                    {
                        apps.Add((fileName, exe));
                        AnsiConsole.MarkupLine($"[grey]Found: {fileName} at {exe}[/]");
                    }
                }
            }
            catch (Exception) { }
        }

        var orderedApps = apps.OrderBy(a => a.Name).ToList();
        AnsiConsole.MarkupLine($"[grey]Found {orderedApps.Count} applications[/]");
        return orderedApps;
    }

    private static bool IsValidAppName(string fileName)
    {
        return !fileName.StartsWith("Microsoft.")
            && !fileName.StartsWith("Windows")
            && !fileName.Equals("cmd", StringComparison.OrdinalIgnoreCase)
            && !fileName.EndsWith(".tmp")
            && !fileName.EndsWith(".cache");
    }

    private static string? SelectAppPath(string appName)
    {
        AnsiConsole.MarkupLine($"[grey]Searching for apps matching '{appName}'...[/]");
        var commonApps = FindCommonApps();
        if (!commonApps.Any())
        {
            AnsiConsole.MarkupLine("[yellow]No applications found. Please enter path manually.[/]");
            return null;
        }

        var matchingApps = commonApps
            .Where(a => a.Name.Contains(appName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        string? appPath = null;

        if (matchingApps.Any())
        {
            var choices = matchingApps.Select(a => $"{a.Name} ({a.Path})").ToList();
            choices.Insert(0, "manual");

            var selection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Select application path (or type 'manual' for custom path):[/]")
                    .PageSize(15)
                    .HighlightStyle(new Style(foreground: Color.Blue))
                    .AddChoices(choices)
            );

            if (selection == "manual")
            {
                return null;
            }

            appPath = matchingApps.First(a => selection.StartsWith($"{a.Name} (")).Path;
        }

        if (appPath == null)
        {
            appPath = AnsiConsole.Ask<string>("[white]App path (press Enter to go back):[/]");
            if (string.IsNullOrWhiteSpace(appPath))
            {
                return null;
            }
        }

        if (!File.Exists(appPath))
        {
            AnsiConsole.MarkupLine(
                $"[red]The file '{appPath}' does not exist. Please provide a valid path.[/]"
            );
            return null;
        }

        return appPath;
    }
    #endregion

    #region Program Entry Point
    static void Main(string[] args)
    {
        // Use the project's root directory for config
        string configPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "config", "config.json")
        );

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
                        Console.ReadKey(true);
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
    #endregion
}
