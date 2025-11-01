using Puppeteer.Console.Constants;
using PuppeteerSharp;
using System.Text.Json;

const string LocalStorageFile = "localStorageData.json";
const string CookiesFile = "cookies.json";
const string TelegramUrl = "https://web.telegram.org/k/";
const string TelegramChatUrl = "https://web.telegram.org/k/#-2294837322";

var hasSession = await SessionFilesExist();
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
        Console.WriteLine(text);
        previousMessage = text;
    }
};

bool loaded = await LoadSession(page);

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

    Console.WriteLine("Chat tab opened, script added.");

    var exitEvent = new TaskCompletionSource();
    Console.CancelKeyPress += async (sender, e) =>
    {
        e.Cancel = true;
        Console.WriteLine("\nStopping browser...");
        await browser.CloseAsync();
        exitEvent.SetResult();
    };

    await exitEvent.Task;
    return;
}

Console.WriteLine("Waiting 60s for manual authentication (if needed)...");
await Task.Delay(60000);

await SaveSession(page);
Console.WriteLine("Ready! You can restart now to reuse session.");

await browser.CloseAsync();
Console.ReadKey();

static async Task SaveSession(IPage page)
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
    await File.WriteAllTextAsync(LocalStorageFile, localStorageJson);

    var cookies = await page.GetCookiesAsync();
    string cookiesJson = JsonSerializer.Serialize(cookies, new JsonSerializerOptions { WriteIndented = true });
    await File.WriteAllTextAsync(CookiesFile, cookiesJson);

    Console.WriteLine("Session data saved!");
}

static async Task<bool> SessionFilesExist() =>
    await Task.Run(() => File.Exists(LocalStorageFile) && File.Exists(CookiesFile));

static async Task<bool> LoadSession(IPage page)
{
    try
    {
        var cookies = JsonSerializer.Deserialize<CookieParam[]>(await File.ReadAllTextAsync(CookiesFile));
        var localStorageData = JsonSerializer.Deserialize<Dictionary<string, string>>(await File.ReadAllTextAsync(LocalStorageFile));

        if (cookies is { Length: > 0 })
            await page.SetCookieAsync(cookies);

        await page.GoToAsync(TelegramUrl, WaitUntilNavigation.DOMContentLoaded);

        if (localStorageData != null)
        {
            await page.EvaluateFunctionAsync(@"data => {
                    for (const key in data) {
                        localStorage.setItem(key, data[key]);
                    }
                }", localStorageData);
        }

        Console.WriteLine("Session data restored!");
        return true;
    }
    catch (Exception)
    {
        Console.WriteLine("No existing session found, please authenticate manually.");
        await page.GoToAsync(TelegramUrl);
        return false;
    }
}