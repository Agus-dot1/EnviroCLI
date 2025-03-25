using Spectre.Console;

namespace EnviroCLI.Utils
{
    public class FindAppHelper
    {
        public static List<(string Name, string Path)> FindCommonApps()
        {
            var apps = new HashSet<(string Name, string Path)>();

            var commonPaths = new[]
            {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData
                )
            ),
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            ),
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData
                ),
                "Programs",
                "Microsoft VS Code"
            ),
            @"C:\Program Files\Google\Chrome\Application",
            @"C:\Program Files\Mozilla Firefox",
            @"C:\Program Files (x86)\Microsoft\Edge\Application",
            @"C:\Program Files\Git\cmd",
            @"C:\Program Files\Docker\Docker\resources",
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData
                ),
                "Discord"
            ),
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData
                ),
                "Slack"
            ),
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
                Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator)
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
    }
}
