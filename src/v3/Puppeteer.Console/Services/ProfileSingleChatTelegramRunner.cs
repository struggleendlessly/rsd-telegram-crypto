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
            System.Console.WriteLine("Press any key to close the window");
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
            var localStorageJson = await page.EvaluateFunctionAsync<string>("() => JSON.stringify(localStorage)");
            if (IsUserLoggedInFromLocalStorage(localStorageJson))
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
        await page.GoToWithDelayAsync(TelegramUrl, GoToMilisecondsDelay);

        var localStorageJson = await page.EvaluateFunctionAsync<string>("() => JSON.stringify(localStorage)");
        await browser.CloseAsync();

        return IsUserLoggedInFromLocalStorage(localStorageJson);
    }

    private static bool IsUserLoggedInFromLocalStorage(string localStorageJson)
    {
        if (string.IsNullOrWhiteSpace(localStorageJson))
            return false;

        try
        {
            using var doc = JsonDocument.Parse(localStorageJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty("number_of_accounts", out var numberOfAccountsProp))
                return false;

            int numberOfAccounts = 0;
            if (numberOfAccountsProp.ValueKind == JsonValueKind.Number && numberOfAccountsProp.TryGetInt32(out var numVal))
            {
                numberOfAccounts = numVal;
            }
            else
            {
                var s = numberOfAccountsProp.GetString();
                if (!int.TryParse(s, out numberOfAccounts))
                    return false;
            }

            if (numberOfAccounts <= 0)
                return false;

            if (!root.TryGetProperty("user_auth", out var userAuthProp))
                return false;

            var userAuthRaw = userAuthProp.GetString();
            if (string.IsNullOrWhiteSpace(userAuthRaw))
                return false;

            using var authDoc = JsonDocument.Parse(userAuthRaw);
            var authRoot = authDoc.RootElement;

            if (!authRoot.TryGetProperty("id", out var idProp))
                return false;

            long userId = 0;
            if (idProp.ValueKind == JsonValueKind.Number && idProp.TryGetInt64(out var idNum))
            {
                userId = idNum;
            }
            else
            {
                var idStr = idProp.GetString();
                if (!long.TryParse(idStr, out userId))
                    return false;
            }

            if (userId <= 0)
                return false;

            return true;
        }
        catch
        {
            return false;
        }
    }
}
