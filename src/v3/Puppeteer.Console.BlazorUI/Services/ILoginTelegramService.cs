namespace Puppeteer.Console.BlazorUI.Services;

public interface ILoginTelegramService
{
    Task<bool> Login(string chatIdChromeProfile);
}