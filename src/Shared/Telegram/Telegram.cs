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
        public Telegram(
            DBContext dBContext,
            IOptions<OptionsTelegram> optionsTelegram)
        {
            this.dBContext = dBContext;
            this.optionsTelegram = optionsTelegram.Value;
        }
        public async Task Start()
        {

        }

        public async Task<bool> SendMessageToGroup(string text)
        {
            var res = false;

            string urlString = $"https://api.telegram.org/bot{optionsTelegram.bot_hash}/" +
                $"sendMessage?" +
                $"message_thread_id={optionsTelegram.message_thread_id_mainfilters}&" +
                $"chat_id={optionsTelegram.chat_id_coins}&" +
                $"text={text}&" +
                $"parse_mode=MarkDown";

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
