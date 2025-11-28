using EnviroCLI.Models;
using Spectre.Console;
using static EnviroCLI.Services.ConfigurationService;
using static EnviroCLI.UI.ConsoleUI;
using static EnviroCLI.Utils.FindAppHelper;
using Color = Spectre.Console.Color;
using Environment = EnviroCLI.Models.Environment;

namespace EnviroCLI.Services
{
    public class PreferenceService
    {
        public static void ManagePreferences(string configPath)
        {
            AnsiConsole.MarkupLine("Enable Preferences\n\n[orange1](The preferences store in the config file but entering the preferences will overwrite the previous settings.)[/]\n\n");
            var config = LoadConfig(configPath);

            var selectedPreferences = AnsiConsole.Prompt(
                new MultiSelectionPrompt<String>()
                    .Title("Select Preferences")
                    .NotRequired()
                    .InstructionsText("Use arrow keys to navigate, [blue]space[/] to toggle and [green]enter[/] to confirm")
                    .AddChoices(new[] { "Enable Tutorial", "Enable Zen mode" })
            );


            config.Tutorial = selectedPreferences.Contains("Enable Tutorial") ? true : false;
            config.ZenMode = selectedPreferences.Contains("Enable Zen mode") ? true : false;

            SaveConfig(configPath, config);
        }
    }
}
