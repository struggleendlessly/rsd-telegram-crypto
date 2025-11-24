using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Puppeteer.Console.BlazorUI;
using Puppeteer.Console.BlazorUI.Services.Implementations;
using PuppeteerSharp;
using RazorConsole.Core;

IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args)
    .UseRazorConsole<Login>();

//AppDomain.CurrentDomain.ProcessExit += OnExit;

hostBuilder.ConfigureServices(services =>
{
    services.AddScoped<ILoginTelegramService, LoginTelegramService>();
    services.AddScoped<IBrowserService, BrowserService>();
    services.AddSingleton<AppStateManager>();

    services.Configure<ConsoleAppOptions>(options =>
    {
        options.AutoClearConsole = false;
    });
});

var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();

IHost host = hostBuilder.Build();

AppServices.Provider = host.Services;

await host.RunAsync();

//static void OnExit(object? sender, EventArgs e)
//{
//    var provider = AppServices.Provider;
    
//    if (provider is null) 
//        return;

//    var state = provider.GetRequiredService<AppStateManager>();

//    if (!state.SaveLoginData)
//    {
//        var browserService = provider.GetRequiredService<IBrowserService>();
//        browserService.ClearBrowserDataAsync().GetAwaiter().GetResult();
//    }
//}