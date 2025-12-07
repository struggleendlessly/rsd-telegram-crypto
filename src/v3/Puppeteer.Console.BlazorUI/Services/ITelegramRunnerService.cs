using Puppeteer.Console.BlazorUI.Models.Dtos;

namespace Puppeteer.Console.BlazorUI.Services;

public interface ITelegramRunnerService
{
    Task RunScrap(bool runOnlyActive);

    List<RunningBrowser> RunningBrowsers { get; }
}
