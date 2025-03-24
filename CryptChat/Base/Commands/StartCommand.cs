using Telegram.Bot.Types;
using Telegram.Bot;

namespace CryptChat.Base.Commands
{
    public class StartCommand : Command
    {
        public override string[] Names { get; set; } = new string[] { "start", "/start" };

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var str = "------------------------------------------------------------------------------------------------" +
                       "\n            C R Y P T C H A T v. 1.0                  " +
                       "\n------------------------------------------------------------------------------------------------" +
                        "\n Команды для работы : " +
                       "\n /lastBlock - начать поиск по валюте";

            await botClient.SendMessage(message.Chat.Id, str);
        }
    }
}
