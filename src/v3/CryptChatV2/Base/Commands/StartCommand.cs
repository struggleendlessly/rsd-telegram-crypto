using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;

namespace CryptChatV2.Base.Commands
{
    public class StartCommand : Command
    {
        public override string[] Names { get; set; } = new string[] { "start", "/start" };

        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var keyboard = new InlineKeyboardMarkup(
                                       new InlineKeyboardButton[][]
                                       {
                                                            new [] {
                                                                InlineKeyboardButton.WithCallbackData("Test1","callback1"),
                                                                InlineKeyboardButton.WithCallbackData("Test2","callback2")
                                                            },
                                       }
                                   );

            var str = "------------------------------------------------------------------------------------------------" +
                       "\n            C R Y P T C H A T v. 1.0                  " +
                       "\n------------------------------------------------------------------------------------------------" +
                        "\n Команды для работы : " +
                       "\n /lastBlock - начать поиск по валюте";

            await botClient.SendMessage(message.Chat.Id, str, replyMarkup: keyboard);
        }
    }
}
