using Microsoft.EntityFrameworkCore;
using Puppeteer.Console.BlazorUI.Constants;
using Puppeteer.Console.BlazorUI.Data;
using Puppeteer.Console.BlazorUI.Helpers;
using Puppeteer.Console.BlazorUI.Models;
using Puppeteer.Console.BlazorUI.Models.Dtos;
using PuppeteerSharp;

namespace Puppeteer.Console.BlazorUI.Services.Implementations;

public class TelegramRunnerService(
    ApplicationDbContext applicationDbContext) : ITelegramRunnerService
{
    public List<RunningBrowser> RunningBrowsers { get; private set; } = [];

    public async Task RunScrap(bool runOnlyActive)
    {
        if (RunningBrowsers.Count != 0)
            return;

        List<TelegramChat> telegramChats = runOnlyActive
            ? await applicationDbContext.TelegramChats.Where(c => c.Active).ToListAsync()
            : await applicationDbContext.TelegramChats.ToListAsync();

        if (telegramChats.Count == 0)
            return;

        foreach (var telegramChat in telegramChats)
        {
            var runningBrowser = await GetBrowserWithRunningScraperForChat(telegramChat.Id, telegramChat.Url);

            if (runningBrowser is null)
                continue;

            var runningBrowserDto = new RunningBrowser(telegramChat.Url, runningBrowser);
            RunningBrowsers.Add(runningBrowserDto);
        }

        System.Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine("Scaper is running for all selected chats");
        System.Console.ResetColor();
    }

    private async Task<IBrowser?> GetBrowserWithRunningScraperForChat(Guid chatId, string chatUrl)
    {
        try
        {
            System.Console.WriteLine($"[{chatUrl}] Starting browser");
            var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = BrowserConstants.HeadlessBrowserArgs,
                UserDataDir = chatId.ToString().ToLower()
            });

            var page = await browser.NewPageAsync();
            
            await page.GoToWithDelayAsync(chatUrl, UserSettingsConstants.GoToMilisecondsDelay);
            await page.AddScriptTagAsync(new AddTagOptions { Path = UserSettingsConstants.ScraperScriptPath });
            await page.AddScriptTagAsync(new AddTagOptions
            {
                Content = @"
                    console.log('[Scraper injected]');
                    const o = console.log;
                    console.log = (...args) => o('[Telegram]', ...args);
                "
            });

            System.Console.WriteLine($"[{chatUrl}] Scraper injected and running.");
            return browser;
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"[{chatUrl}] ERROR: {ex.Message}");
            return null;
        }
    }
}
