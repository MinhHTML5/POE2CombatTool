using System.Text.Json;

namespace POE2RuleTool.Services;

public static class AppSettingsService
{
    private static readonly string SettingsDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "POE2RuleTool");

    private static readonly string SettingsPath = Path.Combine(SettingsDirectory, "settings.json");

    public static string? LoadLastConfigPath()
    {
        try
        {
            if (!File.Exists(SettingsPath))
            {
                return null;
            }

            string json = File.ReadAllText(SettingsPath);
            AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(json);
            return string.IsNullOrWhiteSpace(settings?.LastConfigPath) ? null : settings.LastConfigPath;
        }
        catch
        {
            return null;
        }
    }

    public static void SaveLastConfigPath(string path)
    {
        Directory.CreateDirectory(SettingsDirectory);
        string json = JsonSerializer.Serialize(new AppSettings { LastConfigPath = path }, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsPath, json);
    }

    private sealed class AppSettings
    {
        public string? LastConfigPath { get; set; }
    }
}
