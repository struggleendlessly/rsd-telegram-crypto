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

        Dictionary<string, string> icons = new();
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

            icons.Add("greenBook", "%F0%9F%93%97");
            icons.Add("redBook", "%F0%9F%93%95");
            icons.Add("lightning", "%E2%9A%A1");
            icons.Add("coin", "%F0%9F%AA%99");
            icons.Add("chart", "%F0%9F%93%88");
            icons.Add("whiteCircle", "%E2%9A%AA");
            icons.Add("yellowCircle", "%F0%9F%9F%A1");
            icons.Add("orangeCircle", "%F0%9F%9F%A0");
            icons.Add("redCircle", "%F0%9F%94%B4");
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

        private List<P0_DTO> ProcessDataForMessage(
            List<EthTrainData> ethTrainDatas,
            List<EthBlocks> ethBlocks)
        {
            List<P0_DTO> res = new();

            foreach (var item in ethTrainDatas)
            {
                P0_DTO val = new();

                var block = ethBlocks.FirstOrDefault(x => x.numberInt == item.blockNumberInt);
                int intUnix = Convert.ToInt32(block.timestamp, 16);
                DateTime dateTimeBlock = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTimeBlock = dateTimeBlock.AddSeconds(intUnix);

                val.walletAge = (-1).ToString();
                var age = dateTimeBlock - item.walletCreated;

                if (block is not null)
                {
                    var ageStr = $"{age.Days} d {age.Hours} h {age.Minutes} m";

                    if (age.Days < 0)
                    {
                        ageStr = "-1";
                    }

                    val.walletAge = ageStr;
                }

                string readableTotalSupply = Regex.Replace(item.totalSupply, @"(?<=\d)(?=(\d{3})+$)", ".");

                val.from = item.from;
                val.totalSupply = readableTotalSupply;
                val.ABI = item.ABI;
                val.contractAddress = item.contractAddress;
                val.balanceOnCreating = item.BalanceOnCreating.ToString();
                val.name = item.name;
                val.symbol = item.symbol;
                val.pairAddress = item.pairAddress;

                val.ABIICon = icons["redBook"];
                val.walletIcon = icons["whiteCircle"];
                val.balanceIcon = icons["whiteCircle"];

                if (!string.IsNullOrEmpty(val.ABI))
                {
                    val.ABIICon = icons["greenBook"];
                }

                if (age.Hours > 1)
                {
                    val.walletIcon = icons["yellowCircle"]; // orange 
                }

                if (age.Days > 0)
                {
                    val.walletIcon = icons["orangeCircle"]; // red
                }


                if (age.Days > 7)
                {
                    val.walletIcon = icons["redCircle"]; // red
                }

                if (age.Days > 4000)
                {
                    val.walletIcon = "-1";
                }

                if (item.BalanceOnCreating > 1)
                {
                    val.balanceIcon = icons["yellowCircle"];
                }

                if (item.BalanceOnCreating > 10)
                {
                    val.balanceIcon = icons["orangeCircle"];
                }

                if (item.BalanceOnCreating > 25)
                {
                    val.balanceIcon = icons["redCircle"];
                }

                var text =
                    $"" +
                    $"{icons["lightning"]} [{val.name}({val.symbol})]({optionsTelegram.etherscanUrl}token/{val.contractAddress}) \n" +
                    $"{val.ABIICon}`{val.contractAddress}` \n " +
                    $"{icons["coin"]} `{val.totalSupply}` \n " +
                    $"{val.walletIcon} [{val.walletAge} / {val.balanceIcon} {val.balanceOnCreating} ETH]({optionsTelegram.etherscanUrl}address/{val.from})  \n" +
                    $"";

                val.messageText = text;

                res.Add(val);
            }

            return res;
        }
        public async Task<List<P0_DTO>> SendPO(
            List<EthTrainData> ethTrainDatas,
            List<EthBlocks> ethBlocks)
        {
            List<P0_DTO> collection = ProcessDataForMessage(ethTrainDatas, ethBlocks);
            var threadId = optionsTelegram.message_thread_id_p0;

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
            List<P0_DTO> collection = ProcessDataForMessage(ethTrainDatas, ethBlocks);
            var threadId = optionsTelegram.message_thread_id_p10;

            foreach (var item in collection)
            {
                item.messageText = item.messageText +
                    $"{icons["chart"]} [dextools]({optionsTelegram.dextoolsUrl}app/en/ether/pair-explorer/{item.pairAddress}) " +
                    $"{icons["chart"]} [dexscreener]({optionsTelegram.dexscreenerUrl}ethereum/{item.pairAddress})  \n";
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
