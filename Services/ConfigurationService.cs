using EnviroCLI.Models;
using Spectre.Console;
using System.Text.Json;
using Environment = EnviroCLI.Models.Environment;

namespace EnviroCLI.Services
{
    public class ConfigurationService
    {

        public static readonly JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
        };

        public static Config CreateDefaultConfig(string configPath)
        {
            var newConfig = new Config
            {
                Environment = new List<Environment>(),
                LastUsedEnvironment = null,
            };
            SaveConfig(configPath, newConfig);
            return newConfig;
        }


        public static Config LoadConfig(string configPath)
        {
            try
            {
                if (File.Exists(configPath))
                {
                    var jsonString = File.ReadAllText(configPath);
                    var config = JsonSerializer.Deserialize<Config>(jsonString, options);
                    return config ?? CreateDefaultConfig(configPath);
                }

                return CreateDefaultConfig(configPath);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error loading config: {ex.Message}[/]");
                return new Config { Environment = new List<Environment>(), LastUsedEnvironment = null };
            }
        }

        public static void SaveConfig(string configPath, Config config)
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(config, options);
                File.WriteAllText(configPath, jsonString);
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error saving config: {ex.Message}[/]");
            }
        }
    }
}