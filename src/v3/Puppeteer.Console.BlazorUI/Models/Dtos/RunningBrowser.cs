using PuppeteerSharp;

namespace Puppeteer.Console.BlazorUI.Models.Dtos;

public record RunningBrowser(string CharUrl, IBrowser Browser);
