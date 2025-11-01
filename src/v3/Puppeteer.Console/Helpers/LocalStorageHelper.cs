using PuppeteerSharp;
using System.Text.Json;

namespace Puppeteer.Console.Helpers;

public static class LocalStorageHelper
{
    public static async Task<bool> LoadSession(
        IPage page, 
        string cookiesFilePath, 
        string localStorageFilePath,
        string telegramUrl)
    {
        try
        {
            var cookies = JsonSerializer.Deserialize<CookieParam[]>(await File.ReadAllTextAsync(cookiesFilePath));
            var localStorageData = JsonSerializer.Deserialize<Dictionary<string, string>>(await File.ReadAllTextAsync(localStorageFilePath));

            if (cookies is { Length: > 0 })
                await page.SetCookieAsync(cookies);

            await page.GoToAsync(telegramUrl, WaitUntilNavigation.DOMContentLoaded);

            if (localStorageData is not null)
            {
                await page.EvaluateFunctionAsync(@"data => {
                    for (const key in data) {
                        localStorage.setItem(key, data[key]);
                    }
                }", localStorageData);
            }

            System.Console.WriteLine("Session data restored!");
            return true;
        }
        catch (Exception)
        {
            System.Console.WriteLine("No existing session found, please authenticate manually.");
            await page.GoToAsync(telegramUrl);
            return false;
        }
    }

    public static async Task SaveSession(IPage page, string localStorageFilePath, string cookiesFilePath)
    {
        var localStorageJson = await page.EvaluateFunctionAsync<string>(@"
            () => {
                const json = {};
                for (let i = 0; i < localStorage.length; i++) {
                    const key = localStorage.key(i);
                    json[key] = localStorage.getItem(key);
                }
                return JSON.stringify(json);
            }
        ");
        await File.WriteAllTextAsync(localStorageFilePath, localStorageJson);

        var cookies = await page.GetCookiesAsync();
        string cookiesJson = JsonSerializer.Serialize(cookies, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(cookiesFilePath, cookiesJson);

        System.Console.WriteLine("Session data saved!");
    }
}
