using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Puppeteer.Console.BlazorUI;
using Puppeteer.Console.BlazorUI.Components;
using Puppeteer.Console.BlazorUI.Data;
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
    services.AddScoped<ITelegramRunnerService, TelegramRunnerService>();
    services.AddScoped<IUserSettingsService, UserSettingsService>();

    services.Configure<ConsoleAppOptions>(options =>
    {
        options.AutoClearConsole = false;
    });

    services.AddDbContext<ApplicationDbContext>(options =>
    {
        //string sqlServerConnectionString =
        //    "Data Source=localhost,5533;Initial Catalog=Puppeteer.Console.BlazorUI;User ID=sa;Password=YourStrong!Passw0rd;Encrypt=False;MultipleActiveResultSets=False;TrustServerCertificate=True;Connection Timeout=30;";
        //options.UseSqlServer(sqlServerConnectionString);

        string sqliteConnectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, "Puppeteer.Console.BlazorUI.db")}";
        options.UseSqlite(sqliteConnectionString);
    });
});

var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();

IHost host = hostBuilder.Build();
AppServices.Provider = host.Services;

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

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

    var telegramRunnerService = provider.GetService<ITelegramRunnerService>();

    if (telegramRunnerService is null)
        return;

    if (telegramRunnerService.RunningBrowsers.Count !=0)
        foreach (var rb in telegramRunnerService.RunningBrowsers)
        {
            try
            {
                Console.WriteLine($"[{rb.CharUrl}] Closing browser...");
                rb.Browser.CloseAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing browser [{rb.CharUrl}]: {ex.Message}");
            }
        }
});

await host.RunAsync();