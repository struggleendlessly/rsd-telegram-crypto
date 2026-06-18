namespace Puppeteer.Console.BlazorUI.Models;

public class UserSettings
{
    public int Id { get; set; }
    
    public bool SaveLoginData { get; set; } = true;

    public string UserKey { get; set; } = string.Empty;
}
