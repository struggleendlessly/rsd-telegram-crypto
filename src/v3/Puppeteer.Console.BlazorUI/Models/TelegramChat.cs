namespace Puppeteer.Console.BlazorUI.Models;

public class TelegramChat
{
    public Guid Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public bool Active { get; set; }
}
