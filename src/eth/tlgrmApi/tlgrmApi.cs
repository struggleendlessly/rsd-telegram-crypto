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
                var age = item.walletCreated - dateTimeBlock;

                if (block is not null)
                {
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
                var ABIICon = "❤";
                var walletIcon = "\U0001f9d1‍💻";
                var balanceIcon = "⚪";

                if (!string.IsNullOrEmpty(val.ABI))
                {
                    ABIICon = "💚";
                }

                if (age.TotalDays > 0)
                {
                    walletIcon = "🔴";
                }

                if (item.BalanceOnCreating > 10)
                {
                    balanceIcon = "🔴";
                }

                var text =
                    $"" +
                    $"📌 [{val.name}({val.symbol})]({optionsTelegram.etherscanUrl}token/{val.contractAddress}) \n" +
                    $"{ABIICon}`{val.contractAddress}` \n " +
                    $"💰`{val.totalSupply}` \n " +
                    $"{walletIcon} [{val.walletAge} / {balanceIcon} {val.balanceOnCreating} ETH]({optionsTelegram.etherscanUrl}address/{val.from})  \n" +
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
        public async Task<List<P0_DTO>> SendP1O(
            List<EthTrainData> ethTrainDatas,
            List<EthBlocks> ethBlocks)
        {
            List<P0_DTO> collection = new();
            var threadId = optionsTelegram.message_thread_id_p10;

            foreach (var item in ethTrainDatas)
            {
                P0_DTO val = new();

                var block = ethBlocks.FirstOrDefault(x => x.numberInt == item.blockNumberInt);
                int intUnix = Convert.ToInt32(block.timestamp, 16);
                DateTime dateTimeBlock = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTimeBlock = dateTimeBlock.AddSeconds(intUnix);

                val.walletAge = (-1).ToString();
                var age = item.walletCreated - dateTimeBlock;

                if (block is not null)
                {
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
                val.pairAddress = item.pairAddress;

                var res = 0;
                var ABIICon = "❤";
                var walletIcon = "\U0001f9d1‍💻";
                var balanceIcon = "⚪";

                if (!string.IsNullOrEmpty(val.ABI))
                {
                    ABIICon = "💚";
                }

                if (age.Days > 0)
                {
                    walletIcon = "🔴";
                }
                
                if (item.BalanceOnCreating > 1)
                {
                    balanceIcon = "\t\U0001f7e0";
                }

                if (item.BalanceOnCreating > 10)
                {
                    balanceIcon = "🔴";
                }  

                var text =
                    $"" +
                    $"📌 [{val.name}({val.symbol})]({optionsTelegram.etherscanUrl}token/{val.contractAddress}) \n" +
                    $"{ABIICon}`{val.contractAddress}` \n " +
                    $"💰`{val.totalSupply}` \n " +
                    $"{walletIcon} [{val.walletAge} / {balanceIcon} {val.balanceOnCreating} ETH]({optionsTelegram.etherscanUrl}address/{val.from})  \n" +
                    $"📈 [dextools]({optionsTelegram.dextoolsUrl}app/en/ether/pair-explorer/{val.pairAddress}) \n" +
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
