namespace Puppeteer.Console.BlazorUI.Services;

public interface ILoginTelegramService
{
    Task Login();

    Task<bool> IsLoggedIn();
}