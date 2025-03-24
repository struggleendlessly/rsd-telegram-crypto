using Telegram.Bot.Types;
using Telegram.Bot;
using CryptChat.Base;

namespace CryptChat.Services
{
    public class TelegramBotService
    {
        private readonly TelegramBotClient _botClient;

        public TelegramBotService(string botToken)
        {
            _botClient = new TelegramBotClient(botToken);
        }

        public async Task SetWebhookAsync(string webhookUrl)
        {
            var client = new HttpClient();
            var response = await client.PostAsync(webhookUrl, null);
            Console.WriteLine("Webhook set successfully!");
        }

        public async Task HandleMessageAsync(Message message, Logic logic, CancellationToken cancellationToken)
        {
            var chatId = message.Chat.Id;
            var userId = message.From!.Id;

            bool isAdmin = false;

            if (message.Text != null)
            {
                if (message.Text.StartsWith("/"))
                {
                    isAdmin = await CheckIfUserIsAdmin(chatId, userId);
                }

                if (message.Text == "/start")
                {
                    await logic.StartAsync(message, _botClient);
                }

                if (message.Text == "/lastBlock")
                {
                    var responseMessage = isAdmin ? "Вы админ" : "Вы не админ";
                    await _botClient.SendMessage(chatId, responseMessage, cancellationToken: cancellationToken);
                }
            }
        }

        private async Task<bool> CheckIfUserIsAdmin(long chatId, long userId)
        {
            try
            {
                var administrators = await _botClient.GetChatAdministrators(new ChatId(chatId));
                return administrators.Any(admin => admin.User.Id == userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при проверке админов: {ex.Message}");
                return false;
            }
        }

        /*// Обработка callback-кнопок
async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    await botClient.SendMessage(callbackQuery.Id, "Обработан callback!");
}*/
    }
}
