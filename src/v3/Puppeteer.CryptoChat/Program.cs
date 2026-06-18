using Puppeteer.CryptoChat.Constants;
using Puppeteer.CryptoChat.Responses;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

var builder = WebApplication.CreateBuilder(args);
var token = "8130511485:AAHeCpnorWzj3ZYJPsDo-TE_bGncdqiJlTk";
var webhookUrl = "https://equiponderant-snoutlike-denna.ngrok-free.dev/bot";

builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient(httpClient => new TelegramBotClient(token, httpClient));
builder.Services.AddHttpClient(ApplicationConstants.ApiIdentifier, client =>
{
    client.BaseAddress = new Uri("http://localhost:5018");
});

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/bot/setWebhook", async (TelegramBotClient bot) => { await bot.SetWebhook(webhookUrl); return $"Webhook set to {webhookUrl}"; });
app.MapPost("/bot", OnUpdate);
app.Run();

async void OnUpdate(TelegramBotClient bot, Update update, IHttpClientFactory httpFactory)
{
    if (update.Message is not { } msg)
        return;

    await OnMessage(bot, msg, httpFactory);
}

async Task OnMessage(
    TelegramBotClient bot, 
    Message message,
    IHttpClientFactory httpFactory)
{
    if (message.Text is not { } text)
    {
        Console.WriteLine($"Received a message of type {message.Type}");
        return;
    }

    if (text.StartsWith('/'))
    {
        var space = text.IndexOf(' ');
        if (space < 0) space = text.Length;

        var command = text[..space].ToLower();
        var args = text[space..].TrimStart();

        await OnCommand(bot, command, args, message);
    }
    else
        await OnMenuSelection(bot, text, message, httpFactory);
}

async Task OnMenuSelection(
    TelegramBotClient bot, 
    string text, 
    Message msg,
    IHttpClientFactory httpFactory)
{
    switch (text.ToUpper())
    {
        case "ACTIVATE":
            var client = httpFactory.CreateClient(ApplicationConstants.ApiIdentifier);
            var response = await client.PostAsync("/activate", null);

            if (!response.IsSuccessStatusCode)
            {
                await bot.SendMessage(msg.Chat, "❌ Activation failed");
                return;
            }

            var dto = await response.Content.ReadFromJsonAsync<ApplicationActivationResponseDto>();
            await bot.SendMessage(msg.Chat, $"✅ Activated!\n\nUserKey: `{dto!.UserKey}`", parseMode: ParseMode.Markdown);
            break;

        case "INFO":
            await bot.SendMessage(msg.Chat, "ℹ️ This bot activates CryptoScout.");
            break;

        default:
            await OnCommand(bot, "/start", string.Empty, msg);
            break;
    }
}

async Task OnCommand(TelegramBotClient bot, string command, string args, Message msg)
{
    switch (command)
    {
        case "/start":
            await bot.SendMessage(msg.Chat, "<b><u>Activator for CryptoScout application</u></b>",
                parseMode: ParseMode.Html,
                replyMarkup: new[] { "ACTIVATE", "INFO" });
            break;
    }
}

