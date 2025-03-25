namespace EnviroCLI.Models;
public class Environment
{
    public string? Name { get; set; } = string.Empty;
    public List<App>? Apps { get; set; } = [];
}
