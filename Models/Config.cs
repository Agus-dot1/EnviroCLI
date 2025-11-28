namespace EnviroCLI.Models;

public class Config
{
    public List<Environment>? Environment { get; set; } = new List<Environment>();
    public string? LastUsedEnvironment { get; set; } = string.Empty;
    public bool Tutorial { get; set; } = true;
    public bool ZenMode { get; set; } = false;
}
