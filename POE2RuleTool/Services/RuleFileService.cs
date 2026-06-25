using System.Text.Json;
using System.Text.Json.Serialization;
using POE2RuleTool.Models;

namespace POE2RuleTool.Services;

public static class RuleFileService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static void Save(string path, RuleToolConfig config)
    {
        string json = JsonSerializer.Serialize(config, JsonOptions);
        File.WriteAllText(path, json);
    }

    public static RuleToolConfig Load(string path)
    {
        string json = File.ReadAllText(path);
        using JsonDocument document = JsonDocument.Parse(json);

        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            return new RuleToolConfig
            {
                Rules = JsonSerializer.Deserialize<List<RuleDefinition>>(json, JsonOptions) ?? new List<RuleDefinition>()
            };
        }

        return JsonSerializer.Deserialize<RuleToolConfig>(json, JsonOptions) ?? new RuleToolConfig();
    }
}
