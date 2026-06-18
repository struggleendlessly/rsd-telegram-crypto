namespace Puppeteer.Console.Constants;

public static class BrowserConstants
{
    public static readonly string[] HeadlessBrowserArgs =
    [
        "--disable-background-timer-throttling",
        "--disable-renderer-backgrounding",
        "--disable-backgrounding-occluded-windows",
        "--start-maximized",
        "--disable-web-security",
        "--allow-running-insecure-content",
        "--ignore-certificate-errors"
    ];
}
