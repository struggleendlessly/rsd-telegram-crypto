using Puppeteer.Console.BlazorUI.Constants;
using Puppeteer.Console.BlazorUI.Helpers;
using PuppeteerSharp;
using System.Text.Json;

namespace Puppeteer.Console.BlazorUI.Services.Implementations;

public interface ILoginTelegramService
{
    Task Login();

    Task<bool> IsLoggedIn();
}

public class LoginTelegramService : ILoginTelegramService
{
    public async Task Login()
    {
        var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = false,
            UserDataDir = UserSettingsConstants.UserDataDirectory,
            Args = BrowserConstants.HeadlessBrowserArgs
        });

        var page = await browser.NewPageAsync();
        
        await page.GoToWithDelayAsync(UserSettingsConstants.TelegramBaseUrl, UserSettingsConstants.GoToMilisecondsDelay);
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
        return;
    }

    public async Task<bool> IsLoggedIn()
    {
        var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
            UserDataDir = UserSettingsConstants.UserDataDirectory,
            Args = BrowserConstants.HeadlessBrowserArgs
        });

        var page = await browser.NewPageAsync();
        await page.GoToWithDelayAsync(UserSettingsConstants.TelegramBaseUrl, UserSettingsConstants.GoToMilisecondsDelay);

        var timeoutLoginSeconds = TimeSpan.FromSeconds(1).TotalSeconds;
        bool userLoggedIn = await WaitForUserLogin(page, timeoutLoginSeconds);
        
        await browser.CloseAsync();
        return userLoggedIn;
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
