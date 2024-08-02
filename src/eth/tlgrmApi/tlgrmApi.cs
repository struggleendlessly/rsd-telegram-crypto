using Data.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;
using Shared.Telegram.Models;

using System.Net.Http.Json;
using System.Text.RegularExpressions;

using tlgrmApi.Models;

namespace tlgrmApi
{
    public class tlgrmApi
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly OptionsTelegram optionsTelegram;

        public tlgrmApi(
            ILogger<tlgrmApi> logger,
            IHttpClientFactory httpClient,
            IOptions<OptionsTelegram> options
            )
        {
            this.logger = logger;
            this.optionsTelegram = options.Value;

            this.httpClient = httpClient.CreateClient("Api");
            this.httpClient.BaseAddress = new Uri(optionsTelegram.UrlBase);
        }

        public async Task<int> SendSequest(
            string threadId,
            string text)
        {
            var res = 0;

            string urlString = $"bot{optionsTelegram.bot_hash}/" +
                $"sendMessage?" +
                $"message_thread_id={threadId}&" +
                $"chat_id={optionsTelegram.chat_id_coins}&" +
                $"text={text}&" +
                $"parse_mode=MarkDown&" +
                $"disable_web_page_preview=true";

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            var response = await httpClient.GetFromJsonAsync<MessageSend>(urlString);
            res = response.result.message_id;

            await Task.Delay(optionsTelegram.api_delay_forech);

            return res;
        }

        public async Task<List<P0_DTO>> SendPO(
            List<EthTrainData> ethTrainDatas,
            List<EthBlocks> ethBlocks)
        {
            List<P0_DTO> collection = new();
            var threadId = optionsTelegram.message_thread_id_p0;

            foreach (var item in ethTrainDatas)
            {
                P0_DTO val = new();

                var block = ethBlocks.FirstOrDefault(x => x.numberInt == item.blockNumberInt);
                int intUnix = Convert.ToInt32(block.timestamp, 16);
                DateTime dateTimeBlock = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTimeBlock = dateTimeBlock.AddSeconds(intUnix);

                val.walletAge = (-1).ToString();

                if (block is not null)
                {
                    var age = item.walletCreated - dateTimeBlock;
                    var ageStr = age.ToString(@"d\.hh\:mm");
                    val.walletAge = ageStr;
                }

                string readableTotalSupply = Regex.Replace(item.totalSupply, @"(?<=\d)(?=(\d{3})+$)", "_");

                val.from = item.from;
                val.totalSupply = readableTotalSupply;
                val.ABI = item.ABI;
                val.contractAddress = item.contractAddress;
                val.balanceOnCreating = item.BalanceOnCreating.ToString();
                val.name = item.name;
                val.symbol = item.symbol;

                var res = 0;
                var isABI = "❤";
                if (!string.IsNullOrEmpty(val.ABI))
                {
                    isABI = "💚";
                }

                var text =
                    $"" +
                    $"📌 [{val.name}({val.symbol})]({optionsTelegram.etherscanUrl}token/{val.contractAddress}) \n" +
                    $"{isABI}`{val.contractAddress}` \n " +
                    $"💰`{val.totalSupply}` \n " +
                    $"\U0001f9d1‍💻 [{val.walletAge} / {val.balanceOnCreating} ETH]({optionsTelegram.etherscanUrl}address/{val.from})  \n" +
                    $"";

                val.messageText = text;

                collection.Add(val);
            }

            foreach (var item in collection)
            {
                var t = await SendSequest(threadId, item.messageText);
                item.tlgrmMsgId = t;
            }

            return collection;
        }
    }
}
