using Puppeteer.Console.BlazorUI.Constants;
using Puppeteer.Console.BlazorUI.Helpers;
using PuppeteerSharp;

namespace Puppeteer.Console.BlazorUI.Services.Implementations;

public class BrowserService : IBrowserService
{
    public async Task ClearBrowserDataAsync()
    {
        var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            UserDataDir = UserSettingsConstants.UserDataDirectory
        });

        var page = await browser.NewPageAsync();
        await page.GoToWithDelayAsync(UserSettingsConstants.TelegramBaseUrl, UserSettingsConstants.GoToMilisecondsDelay);

        var client = await page.CreateCDPSessionAsync();

        await client.SendAsync("Storage.clearDataForOrigin", new
        {
            origin = "https://web.telegram.org",
            storageTypes = "all"
        });

        await client.SendAsync("Network.clearBrowserCache");
        await client.SendAsync("Network.clearBrowserCookies");
        await browser.CloseAsync();
    }
}
