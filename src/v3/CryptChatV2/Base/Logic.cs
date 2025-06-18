using Telegram.Bot.Types;
using Telegram.Bot;
using CryptChatV2.Base.Commands;

namespace CryptChatV2.Base
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
