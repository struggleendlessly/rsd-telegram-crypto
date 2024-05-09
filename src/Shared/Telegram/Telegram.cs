using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;
using Shared.DB;

using System.Net;

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



        public async Task<bool> SendMessageToGroup(string text)
        {
            var res = false;

            string urlString = $"https://api.telegram.org/bot{optionsTelegram.bot_hash}/" +
                $"sendMessage?" +
                $"message_thread_id={message_thread_id}&" +
                $"chat_id={optionsTelegram.chat_id_coins}&" +
                $"text={text}&" +
                $"parse_mode=MarkDown&" +
                $"disable_web_page_preview=true";

            using (var webclient = new WebClient())
            {
                var response = await webclient.DownloadStringTaskAsync(urlString);
                res = true;
            }

            await Task.Delay(optionsTelegram.api_delay_forech);


            return res;
        }
    }
}
