using CryptChatV2.Base;
using CryptChatV2.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);
var botToken = "8130511485:AAHeCpnorWzj3ZYJPsDo-TE_bGncdqiJlTk";
var botService = new TelegramBotService(botToken);
var ngrokUrl = await NgrokService.GetNgrokUrlAsync();
// Port = 443 - port fowarding (5050). Firewall rules, income input yes allow.
var webhookUrl = $"https://api.telegram.org/bot{botToken}/setWebhook?url={ngrokUrl}/webhook";

await botService.SetWebhookAsync(webhookUrl);

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var app = builder.Build();
app.UseHttpsRedirection();

app.MapPost("/webhook", async ([FromBody] Update update) =>
{
    if (update.Message is { } message)
    {
        var logic = new Logic();
        await botService.HandleMessageAsync(message, logic, CancellationToken.None);
    }
    /*    else if (update.CallbackQuery is { } callbackQuery)
    {
        await HandleCallbackQuery(botClient, callbackQuery);
    }*/
});

app.Run();