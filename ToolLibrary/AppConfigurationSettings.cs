using System.Text.Json.Serialization;


namespace ToolLibrary;

public class AppConfigurationSettings
{
    [JsonPropertyName("Integer variables")]
    public Dictionary<string, int>? IntVariables { get; init; }

    public Dictionary<string, string>? ConnectionStrings { get; init; }

    [JsonPropertyName("Serilog")]
    public Dictionary<string, object>? SerilogSettings { get; init; }
}
