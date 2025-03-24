using Telegram.Bot.Types;
using Telegram.Bot;

namespace CryptChat.Base.Commands
{
    public abstract class Command
    {
        public abstract string[] Names { get; set; }

        public abstract Task Execute(Message message, TelegramBotClient client);

        public bool Contains(string message)
        {
            foreach (var mess in Names)
            {
                if (message != null && message.Contains(mess))
                {
                    return true;
                }
            }
            return false;
        }
    }
}