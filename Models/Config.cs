namespace EnviroCLI.Models;
public class Config
{
    public List<Environment>? Environment { get; set; } = new List<Environment>();
    public string? LastUsedEnvironment { get; set; } = string.Empty;
}
