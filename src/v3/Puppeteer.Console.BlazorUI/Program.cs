using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Puppeteer.Console.BlazorUI;
using Puppeteer.Console.BlazorUI.Components;
using Puppeteer.Console.BlazorUI.Services;
using Puppeteer.Console.BlazorUI.Services.Implementations;
using PuppeteerSharp;
using RazorConsole.Core;

IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
    .UseRazorConsole<Main>();

hostBuilder.ConfigureServices(services =>
{
    services.AddScoped<ILoginTelegramService, LoginTelegramService>();
    services.AddScoped<IBrowserService, BrowserService>();
    services.AddSingleton<IUserSettingsService, UserSettingsService>();

    services.Configure<ConsoleAppOptions>(options =>
    {
        options.AutoClearConsole = false;
    });
});

var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();

IHost host = hostBuilder.Build();
AppServices.Provider = host.Services;

var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStopped.Register(() =>
{
    var provider = AppServices.Provider;

    if (provider is null)
        return;

    var userSettingsService = provider.GetService<IUserSettingsService>();

    if (userSettingsService is null) 
        return;

    if (!userSettingsService.Settings.SaveLoginData)
    {
        var browserService = provider.GetRequiredService<IBrowserService>();
        browserService.ClearBrowserDataAsync().GetAwaiter().GetResult();
    }
});

await host.RunAsync();