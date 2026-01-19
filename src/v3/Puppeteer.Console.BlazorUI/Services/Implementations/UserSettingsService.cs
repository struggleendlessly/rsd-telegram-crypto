using Puppeteer.Console.BlazorUI.Models;
using System.Text.Json;

namespace Puppeteer.Console.BlazorUI.Services.Implementations;

public class UserSettingsService : IUserSettingsService
{
    private readonly string _filePath = "settings.json";

    public UserSettings Settings { get; private set; }

    public UserSettingsService()
    {
        if (File.Exists(_filePath))
        {
            var json = File.ReadAllText(_filePath);
            Settings = JsonSerializer.Deserialize<UserSettings>(json)!;
        }
        else
        {
            Settings = new UserSettings();
            Save();
        }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }
}
