using CryptChat.Base.Commands;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace CryptChat.Base
{
    public class Logic
    {
        public async Task StartAsync(Message message, TelegramBotClient client)
        {
            StartCommand start = new StartCommand();
            await start.Execute(message, client);
        }
    }
}
