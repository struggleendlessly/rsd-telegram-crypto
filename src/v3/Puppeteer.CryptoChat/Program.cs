using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

var builder = WebApplication.CreateBuilder(args);
var token = "8130511485:AAHeCpnorWzj3ZYJPsDo-TE_bGncdqiJlTk";
var webhookUrl = "https://equiponderant-snoutlike-denna.ngrok-free.dev/bot";

builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient(httpClient => new TelegramBotClient(token, httpClient));

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/bot/setWebhook", async (TelegramBotClient bot) => { await bot.SetWebhook(webhookUrl); return $"Webhook set to {webhookUrl}"; });
app.MapPost("/bot", OnUpdate);
app.Run();

async void OnUpdate(TelegramBotClient bot, Update update)
{
    if (update.Message is not { } msg)
        return;

    await OnMessage(bot, msg, update.Type);
}

async Task OnMessage(
    TelegramBotClient bot, 
    Message message, 
    UpdateType type)
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
        await OnTextMessage(bot, message);
}

async Task OnTextMessage(TelegramBotClient bot, Message msg)
{
    Console.WriteLine($"Received text '{msg.Text}' in {msg.Chat}");
    await OnCommand(bot, "/start", string.Empty, msg);
}

async Task OnCommand(TelegramBotClient bot, string command, string args, Message msg)
{
    Console.WriteLine($"Received command: {command} {args}");

    switch (command)
    {
        case "/start":
            await bot.SendMessage(msg.Chat, """
                <b><u>Bot menu</u></b>:
                /photo [url]
                /inline_buttons
                /keyboard
                /remove
                /poll
                /reaction
                """,
                parseMode: ParseMode.Html,
                replyMarkup: new ReplyKeyboardRemove());
            break;
    }
}

