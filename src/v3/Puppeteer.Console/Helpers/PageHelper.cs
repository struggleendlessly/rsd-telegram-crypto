using PuppeteerSharp;

namespace Puppeteer.Console.Helpers;

public static class PageHelper
{
    public static async Task GoToWithDelayAsync(this IPage page, string url, int milisecondsDelay)
    {
        await page.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded);
        await Task.Delay(milisecondsDelay);
    }
}
