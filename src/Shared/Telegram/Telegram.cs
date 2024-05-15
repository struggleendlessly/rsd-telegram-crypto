using Azure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;
using Shared.DB;
using Shared.Telegram.Models;

using System.Net;
using System.Text.Json;

using TL;

using WTelegram;

namespace Shared.Telegram
{
    public class Telegram
    {
        private readonly DBContext dBContext;
        private readonly OptionsTelegram optionsTelegram;
        private string message_thread_id;
        public Telegram(
            DBContext dBContext,
            IOptions<OptionsTelegram> optionsTelegram)
        {
            this.dBContext = dBContext;
            this.optionsTelegram = optionsTelegram.Value;
            SetGroup();
        }
        public void SetGroup(int v = 0)
        {
            switch (v)
            {
                case 1:
                    message_thread_id = optionsTelegram.message_thread_id_healthCheck;
                    break;
                default:
                    message_thread_id = optionsTelegram.message_thread_id_mainfilters;
                    break;
            }
        }



        public async Task<int> SendMessageToGroup(string text, string threadId)
        {
            var res = 0;

            string urlString = $"https://api.telegram.org/bot{optionsTelegram.bot_hash}/" +
                $"sendMessage?" +
                $"message_thread_id={threadId}&" +
                $"chat_id={optionsTelegram.chat_id_coins}&" +
                $"text={text}&" +
                $"parse_mode=MarkDown&" +
                $"disable_web_page_preview=true";

            using (var webclient = new WebClient())
            {
                var response = await webclient.DownloadStringTaskAsync(urlString);

                var t = JsonSerializer.Deserialize<MessageSend>(response);
                res = t.result.message_id;
            }

            await Task.Delay(optionsTelegram.api_delay_forech);


            return res;
        }

        public async Task<int> DeleteMessageInGroup(int messageId, string threadId)
        {
            var res = 0;

            string urlString = $"https://api.telegram.org/bot{optionsTelegram.bot_hash}/" +
                $"deleteMessage?" +
                $"message_thread_id={threadId}&" +
                $"chat_id={optionsTelegram.chat_id_coins}&" +
                $"message_id={messageId}";

            using (var webclient = new WebClient())
            {
                try
                {
                    var response = await webclient.DownloadStringTaskAsync(urlString);
                }
                catch (Exception ex)
                {

                }
            }

            await Task.Delay(optionsTelegram.api_delay_forech);

            return res;
        }
    }
}
