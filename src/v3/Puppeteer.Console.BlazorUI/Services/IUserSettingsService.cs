using Puppeteer.Console.BlazorUI.Models;

namespace Puppeteer.Console.BlazorUI.Services;

public interface IUserSettingsService
{
    UserSettings Settings { get; }

    void Save();
}
