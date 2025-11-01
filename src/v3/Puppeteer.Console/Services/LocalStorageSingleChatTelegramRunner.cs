using Puppeteer.Console.Constants;
using Puppeteer.Console.Helpers;
using PuppeteerSharp;

namespace Puppeteer.Console.Services;

public class LocalStorageSingleChatTelegramRunner
{
    private const string LocalStorageFile = "localStorageData.json";
    private const string CookiesFile = "cookies.json";
    private const string TelegramUrl = "https://web.telegram.org/k/";
    private const string TelegramChatUrl = "https://web.telegram.org/k/#-2294837322";

    public async Task RunAsync()
    {
        var hasSession = await FileHelper.SessionFilesExist(LocalStorageFile, CookiesFile);
        var hasSessionMessage = hasSession ? "Session found — running in headless mode." : "No session found — running in visible mode.";

        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        var browserLaunchOptions = new LaunchOptions
        {
            Headless = hasSession,
            Args = BrowserConstants.HeadlessBrowserArgs
        };

        var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(browserLaunchOptions);
        var page = await browser.NewPageAsync();

        string previousMessage = string.Empty;
        page.Console += (sender, e) =>
        {
            const string tag = "[CHAT MESSAGE]";
            string text = e.Message.Text;
            if (text.Contains(tag) && text != previousMessage)
            {
                System.Console.WriteLine(text);
                previousMessage = text;
            }
        };

        bool loaded = await LocalStorageHelper.LoadSession(page, CookiesFile, LocalStorageFile, TelegramUrl);

        if (loaded)
        {
            await Task.Delay(10000);
            await page.GoToAsync(TelegramChatUrl, WaitUntilNavigation.DOMContentLoaded);

            await page.AddScriptTagAsync(new AddTagOptions { Path = "./Scripts/scraper_script.js" });

            await page.AddScriptTagAsync(new AddTagOptions
            {
                Content = @"
                    console.log('[Custom Logger] Script injected successfully!');
                    const originalLog = console.log;
                    console.log = (...args) => {
                        originalLog('[Telegram]', ...args);
                    };
                "
            });

            System.Console.WriteLine("Chat tab opened, script added.");

            var exitEvent = new TaskCompletionSource();
            System.Console.CancelKeyPress += async (sender, e) =>
            {
                e.Cancel = true;
                System.Console.WriteLine("\nStopping browser...");
                await browser.CloseAsync();
                exitEvent.SetResult();
            };

            await exitEvent.Task;
            return;
        }

        System.Console.WriteLine("Waiting 60s for manual authentication (if needed)...");
        await Task.Delay(60000);

        await LocalStorageHelper.SaveSession(page, LocalStorageFile, CookiesFile);
        System.Console.WriteLine("Ready! You can restart now to reuse session.");

        await browser.CloseAsync();
        System.Console.ReadKey();
    }
}
