using Puppeteer.Console.Constants;
using Puppeteer.Console.Helpers;
using PuppeteerSharp;
using System.Text.Json;

namespace Puppeteer.Console.Services;

public class ProfileSingleChatTelegramRunner
{
    private const string TelegramUrl = "https://web.telegram.org/k/";
    private const string TelegramChatUrl = "https://web.telegram.org/k/#-2294837322";
    private const string UserDataDir = "telegram-profile";
    private const int GoToMilisecondsDelay = 10000;

    public async Task RunAsync()
    {
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        bool loggedIn = await CheckIfUserLoggedIn();
        string loggedInMessage = loggedIn 
            ? "Telegram session found. Launching headless..." 
            : "No session found. Opening browser for manual login...";

        System.Console.WriteLine(loggedInMessage);

        var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = loggedIn,
            UserDataDir = UserDataDir,
            Args = BrowserConstants.HeadlessBrowserArgs
        });

        var page = await browser.NewPageAsync();

        if (!loggedIn)
        {
            await page.GoToWithDelayAsync(TelegramUrl, GoToMilisecondsDelay);
            System.Console.WriteLine("Please log in to Telegram Web...");
            System.Console.WriteLine("Waiting for authentication...");

            var timeoutLoginSeconds = TimeSpan.FromMinutes(5).TotalSeconds; 
            bool userLoggedIn = await WaitForUserLogin(page, timeoutLoginSeconds);

            if (userLoggedIn)
                System.Console.WriteLine("Login detected, closing browser...");
            else
                System.Console.WriteLine("Login timeout. Please try again.");

            await browser.CloseAsync();
            System.Console.ReadKey();
            return;
        }

        await page.GoToWithDelayAsync(TelegramChatUrl, GoToMilisecondsDelay);
        
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

        System.Console.WriteLine("Chat tab opened, script added. Press Ctrl + C to exit.");

        var exitEvent = new TaskCompletionSource();
        System.Console.CancelKeyPress += async (sender, e) =>
        {
            e.Cancel = true;
            System.Console.WriteLine("\nStopping browser...");
            await browser.CloseAsync();
            exitEvent.SetResult();
        };

        await exitEvent.Task;
    }

    private static async Task<bool> WaitForUserLogin(IPage page, double timeoutSeconds)
    {
        int elapsed = 0;
        while (elapsed < timeoutSeconds)
        {
            bool loggedIn = await page.EvaluateFunctionAsync<bool>(@"
                () => {
                    const value = localStorage.getItem('number_of_accounts');
                    if (!value) return false;
                
                    const parsed = parseInt(value);
                    if (isNaN(parsed)) return false;

                    return parsed > 0;
                }
            ");

            if (loggedIn)
                return true;

            await Task.Delay(2000);
            elapsed += 2;
        }

        return false;
    }

    private static async Task<bool> CheckIfUserLoggedIn()
    {
        await using var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            UserDataDir = UserDataDir
        });

        var page = await browser.NewPageAsync();
        //doesn't work sometimes. We need to add an additional check, using other parameters from local storage
        await page.GoToWithDelayAsync(TelegramUrl, (int)TimeSpan.FromSeconds(3).TotalMilliseconds); 

        var localStorageJson = await page.EvaluateFunctionAsync<string>("() => JSON.stringify(localStorage)");
        await browser.CloseAsync();

        if (string.IsNullOrEmpty(localStorageJson))
            return false;

        try
        {
            using var doc = JsonDocument.Parse(localStorageJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty("number_of_accounts", out var numberOfAccounts))
                return false;

            if (!int.TryParse(numberOfAccounts.ToString(), out var numberOfAccountsParsed))
                return false;

            if (numberOfAccountsParsed <= 0)
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}
