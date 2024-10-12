using Data;
using Data.Models;

using etherscan;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Nethereum.Util;

using Newtonsoft.Json;

using Shared.ConfigurationOptions;
using Shared.DTO;
using Shared.Telegram.Models;

using System.Globalization;
using System.Net.Http.Json;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Web;

using tlgrmApi.Models;

using static System.Net.Mime.MediaTypeNames;

namespace tlgrmApi
{
    public class tlgrmApi
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly dbContext dbContext;
        private readonly OptionsTelegram optionsTelegram;
        private readonly EtherscanApi etherscanApi;
        Random rnd = new Random();

        Dictionary<string, string> icons = new();
        List<WalletNames> walletNames = new();

        public tlgrmApi(
            ILogger<tlgrmApi> logger,
            IHttpClientFactory httpClient,
            dbContext dbContext,
            EtherscanApi etherscanApi,
            IOptions<OptionsTelegram> options
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.optionsTelegram = options.Value;
            this.etherscanApi = etherscanApi;

            this.httpClient = httpClient.CreateClient("Api");
            this.httpClient.BaseAddress = new Uri(optionsTelegram.UrlBase);

            icons.Add("greenBook", "%F0%9F%93%97");
            icons.Add("redBook", "%F0%9F%93%95");
            icons.Add("lightning", "%E2%9A%A1");
            icons.Add("coin", "%F0%9F%AA%99");
            icons.Add("chart", "%F0%9F%92%B9");
            icons.Add("whiteCircle", "%E2%9A%AA");
            icons.Add("yellowCircle", "%F0%9F%9F%A1");
            icons.Add("orangeCircle", "%F0%9F%9F%A0");
            icons.Add("redCircle", "%F0%9F%94%B4");
            icons.Add("star", "%E2%9C%A8");
            icons.Add("snowflake", "%E2%9C%B3");
            icons.Add("poops", "%F0%9F%92%A9");
            icons.Add("rocket", "%F0%9F%9A%80");
            icons.Add("flagRed", "%F0%9F%9A%A9");
            icons.Add("buy", "%F0%9F%93%88");
            icons.Add("sell", "%F0%9F%93%89");
            icons.Add("squareYellow", "%F0%9F%9F%A8");
            icons.Add("squareOrange", "%F0%9F%9F%A7");
            icons.Add("squareRed", "%F0%9F%9F%A5");
            icons.Add("fire", "%F0%9F%94%A5");
            icons.Add("boom", "%F0%9F%92%A5");
            icons.Add("clockSend", "%E2%8F%B3");
            icons.Add("calendar", "%F0%9F%97%93");
            icons.Add("antenna", "%F0%9F%93%B6");
            icons.Add("SCROLL", "%F0%9F%93%9C");

            walletNames = dbContext.WalletNames.ToList();
        }

        public async Task<long> SendSequest(
            string threadId,
            string text,
            string chat_id)
        {
            var res = 0L;

            var bot_hashIndex = rnd.Next(0, optionsTelegram.bot_hash.Count - 1);

            string urlString = $"bot{optionsTelegram.bot_hash[bot_hashIndex]}/" +
                $"sendMessage?" +
                $"message_thread_id={threadId}&" +
                $"chat_id={chat_id}&" +
                $"text={text}&" +
                $"parse_mode=MarkDown&" +
                $"disable_web_page_preview=true";

            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            var ee = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, urlString));
            var nn = ee.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<MessageSend>(nn.Result);
            //var response = await httpClient.GetFromJsonAsync<MessageSend>(urlString);
            res = resp.result.message_id;

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

                val.EthTrainData = item;

                if (item.EthSwapEvents.Any(x => x.tokenNotEth != ""))
                {
                    val.currency = "XXX";
                }

                var block = ethBlocks.FirstOrDefault(x => x.numberInt == item.blockNumberInt);
                int intUnix = Convert.ToInt32(block.timestamp, 16);
                DateTime dateTimeBlock = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTimeBlock = dateTimeBlock.AddSeconds(intUnix);
                val.EthTrainDataId = item.Id;

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

                var sourceWalletName = "Wallet";
                var sourceWalletIcon = icons["star"];
                if (item.WalletSource1inCountRemLiq > 0)
                {
                    sourceWalletIcon = icons["poops"];
                }

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

                var isWalletKnown = walletNames.FirstOrDefault(x => x.Address.Equals(item.WalletSource1in, StringComparison.InvariantCultureIgnoreCase));

                if (isWalletKnown is not null)
                {
                    sourceWalletName = $"{isWalletKnown.Name} Wallet";
                    sourceWalletIcon = icons["snowflake"];
                }
                val.line_tokenName =$"{icons["lightning"]} [{HttpUtility.UrlEncode(val.name)}({HttpUtility.UrlEncode(val.symbol)})]({optionsTelegram.etherscanUrl}token/{val.contractAddress})";
                val.line_tokenAddress = $"{val.ABIICon}`{val.contractAddress}`";
                val.line_tokenSupply = $"{icons["coin"]} `{val.totalSupply}`";
                val.line_WalletAgeAndBalance = $"{val.walletIcon} [{val.walletAge}  {val.balanceIcon} {val.balanceOnCreating} {val.currency}]({optionsTelegram.etherscanUrl}address/{val.from})";
                val.line_WalletFromType = $"{sourceWalletIcon} [{sourceWalletName}]({optionsTelegram.etherscanUrl}address/{item.WalletSource1in}): {item.WalletSource1inCountRemLiq}";

                //var text =
                //    $"" +
                //    $"{icons["lightning"]} [{val.name}({val.symbol})]({optionsTelegram.etherscanUrl}token/{val.contractAddress}) \n" +
                //    $"{val.ABIICon}`{val.contractAddress}` \n " +
                //    $"{icons["coin"]} `{val.totalSupply}` \n " +
                //    $"{val.walletIcon} [{val.walletAge}  {val.balanceIcon} {val.balanceOnCreating} {val.currency}]({optionsTelegram.etherscanUrl}address/{val.from})  \n" +
                //    $"{sourceWalletIcon} [{sourceWalletName}]({optionsTelegram.etherscanUrl}address/{item.WalletSource1in}): {item.WalletSource1inCountRemLiq}  \n" +
                //    $"";

                //val.messageText = text;

                res.Add(val);
            }

            return res;
        }
        public async Task<List<P0_DTO>> SendPO(
            List<EthTrainData> ethTrainDatas,
            List<EthBlocks> ethBlocks,
            string env = ""
            )
        {
            List<P0_DTO> collection = ProcessDataForMessage(ethTrainDatas, ethBlocks);
            var threadId = "";
            var chat_id = "";

            switch (env)
            {
                case "public":
                    threadId = optionsTelegram.public_message_thread_id_p0_newTokens;
                    chat_id = optionsTelegram.public_chat_id_coins;
                    break;

                case "closed":
                    threadId = optionsTelegram.closed_message_thread_id_p0_newTokens;
                    chat_id = optionsTelegram.closed_chat_id_coins;
                    break;

                default:
                    threadId = optionsTelegram.message_thread_id_p0; 
                    chat_id = optionsTelegram.chat_id_coins;
                    break;
            }

            foreach (var item in collection)
            {
                switch (env)
                {
                    //case "":
                    //    item.messageText =
                    //        item.line_tokenName + " \n" +
                    //        item.line_tokenAddress + " \n" +
                    //        item.line_tokenSupply + " \n" +
                    //        item.line_WalletAgeAndBalance + " \n" +
                    //        item.line_WalletFromType;
                    //    break;

                    default:
                        item.messageText =
                            item.line_tokenName + " \n" +
                            item.line_tokenAddress + " \n" +
                            item.line_tokenSupply + " \n" +
                            item.line_WalletAgeAndBalance + " \n" +
                            item.line_WalletFromType;
                        break;
                }

                var t = await SendSequest(threadId, item.messageText, chat_id);
                item.tlgrmMsgId = t;
            }

            return collection;
        }
        public async Task<List<P0_DTO>> SendP1O(
            List<EthTrainData> ethTrainDatas,
            List<EthBlocks> ethBlocks,
            string env = "")
        {
            List<P0_DTO> collection = ProcessDataForMessage(ethTrainDatas, ethBlocks);

            var threadId = "";
            var chat_id = "";

            switch (env)
            {
                case "public":
                    threadId = optionsTelegram.public_message_thread_id_p10_livePairs;
                    chat_id = optionsTelegram.public_chat_id_coins;
                    break;

                case "closed":
                    threadId = optionsTelegram.closed_message_thread_id_p10_livePairs;
                    chat_id = optionsTelegram.closed_chat_id_coins;
                    break;

                default:
                    threadId = optionsTelegram.message_thread_id_p10;
                    chat_id = optionsTelegram.chat_id_coins;
                    break;
            }

            foreach (var item in collection)
            {
                item.messageText =
                    item.line_tokenName + " \n" +
                    item.line_tokenAddress + " \n" +
                    item.line_tokenSupply + " \n" +
                    item.line_WalletAgeAndBalance + " \n" +
                    item.line_WalletFromType + " \n" +
                    $"{icons["chart"]} [dextools]({optionsTelegram.dextoolsUrl}app/en/ether/pair-explorer/{item.pairAddress}) " +
                    $"{icons["chart"]} [dexscreener]({optionsTelegram.dexscreenerUrl}ethereum/{item.pairAddress})  \n";
            }

            foreach (var item in collection)
            {
                var t = await SendSequest(threadId, item.messageText, chat_id);
                item.tlgrmMsgId = t;
            }

            return collection;
        }

        public async Task<List<P0_DTO>> SendP20(
            List<EthTrainData> ethTrainDatas,
            List<EthBlocks> ethBlocks,
            List<EthTokensVolumeAvarageDTO> validated,
            List<EthTokensVolume> volumeRiseCountList,
            int message_thread_id_p20mins,
            string addition = "",
            string env = "")
        {
            var ethPrice = await etherscanApi.getEthPrice();
            List<P0_DTO> collection = ProcessDataForMessage(ethTrainDatas, ethBlocks);

            var threadId = "";
            var chat_id = "";

            switch (env)
            {
                case "public":
                    chat_id = optionsTelegram.public_chat_id_coins;
                    break;

                case "closed":
                    chat_id = optionsTelegram.closed_chat_id_coins;
                    break;

                default:
                    chat_id = optionsTelegram.chat_id_coins;
                    break;
            }

            switch (message_thread_id_p20mins)
            {
                case 1:
                    threadId = optionsTelegram.message_thread_id_p23_1mins;
                    break;
                case 5:
                    switch (env)
                    {
                        case "public":
                            threadId = optionsTelegram.public_message_thread_id_p22_5mins;

                            break;

                        case "closed":
                            threadId = optionsTelegram.closed_message_thread_id_p22_5mins;
                            break;

                        default:
                            threadId = optionsTelegram.message_thread_id_p22_5mins;
                            break;
                    }

                    if (addition.Equals("5mins_03v100mc"))
                    {
                        switch (env)
                        {
                            case "public":
                                threadId = optionsTelegram.public_message_thread_id_p25_5mins_v03_mc0to100k;
                                break;

                            case "closed":
                                threadId = optionsTelegram.closed_message_thread_id_p25_5mins_v03_mc0to100k;
                                break;

                            default:
                                threadId = optionsTelegram.message_thread_id_p25_5mins_v03_mc0to100k;
                                break;
                        }
                    }

                    if (addition.Equals("5mins_09v01_1mc"))
                    {
                        switch (env)
                        {
                            case "public":
                                threadId = optionsTelegram.public_message_thread_id_p26_5mins_v09_mc100kto1m;
                                break;

                            case "closed":
                                threadId = optionsTelegram.closed_message_thread_id_p26_5mins_v09_mc100kto1m;
                                break;

                            default:
                                threadId = optionsTelegram.message_thread_id_p26_5mins_v09_mc100kto1m;
                                break;
                        }
                    }

                    break;
                case 30:
                    switch (env)
                    {
                        case "public":
                            threadId = optionsTelegram.public_message_thread_id_p21_30mins;
                            break;

                        case "closed":
                            threadId = optionsTelegram.closed_message_thread_id_p21_30mins;
                            break;

                        default:
                            threadId = optionsTelegram.message_thread_id_p21_30mins;
                            break;
                    }

                    if (addition.Equals("30mins_03v100mc"))
                    {
                        switch (env)
                        {
                            case "public":
                                threadId = optionsTelegram.public_message_thread_id_p212_30mins_v03_mc0to100k;
                                break;

                            case "closed":
                                threadId = optionsTelegram.closed_message_thread_id_p212_30mins_v03_mc0to100k;
                                break;

                            default:
                                threadId = optionsTelegram.message_thread_id_p212_30mins_v03_mc0to100k;
                                break;
                        }
                    }

                    if (addition.Equals("30mins_09v01_1mc"))
                    {
                        switch (env)
                        {
                            case "public":
                                threadId = optionsTelegram.public_message_thread_id_p28_30mins_v09_mc100kto1m;
                                break;

                            case "closed":
                                threadId = optionsTelegram.closed_message_thread_id_p28_30mins_v09_mc100kto1m;
                                break;

                            default:
                                threadId = optionsTelegram.message_thread_id_p28_30mins_v09_mc100kto1m;
                                break;
                        }
                    }

                    break;
                case 60:
                    switch (env)
                    {
                        case "public":
                            threadId = optionsTelegram.public_message_thread_id_p20_60mins;
                            break;

                        case "closed":
                            threadId = optionsTelegram.closed_message_thread_id_p20_60mins;
                            break;

                        default:
                            threadId = optionsTelegram.message_thread_id_p20_60mins;
                            break;
                    }

                    break;
            }


            foreach (var item in collection)
            {
                var average = validated.FirstOrDefault(x => x.EthTrainDataId == item.EthTrainDataId);

                decimal x = 99999;

                if (average.volumePositiveEthAverage != 0)
                {
                    x = (decimal)(average.last.volumePositiveEth / average.volumePositiveEthAverage);
                }

                string xxx = icons["squareYellow"];

                if (x > 5)
                {
                    xxx = icons["squareOrange"];
                }

                if (x > 10)
                {
                    xxx = icons["squareRed"];
                }

                string buyToSell = icons["fire"];

                if (average.last.volumeNegativeEth == 0 || average.last.volumePositiveEth / average.last.volumeNegativeEth >= 2)
                {
                    buyToSell = icons["boom"];
                }

                var totalSupply = BigDecimal.Parse(item.EthTrainData.totalSupply);
                var marketCap = totalSupply * (BigDecimal)item.EthTrainData.EthSwapEvents.FirstOrDefault().priceEth * (BigDecimal)ethPrice;
                var ee = marketCap.ToString().Split('.')[0];
                var marketCapStr = Regex.Replace(BigInteger.Parse(ee).ToString(), @"(?<=\d)(?=(\d{3})+$)", ".");
                var volumeRiseCount = volumeRiseCountList.Where(x => x.EthTrainDataId == item.EthTrainDataId).ToList();

                switch (env)
                {
                    case "public":
                        item.messageText =
                            item.line_tokenName + " \n" +
                            item.line_tokenAddress + " \n" +

                            $"{icons["clockSend"]} {average.periodInMins} mins   \n" +
                            $"{buyToSell} Now buy:  {(decimal)average.last.volumePositiveEth:0.##} {item.currency}  Sell:  {(decimal)average.last.volumeNegativeEth:0.##} {item.currency}  \n" +
                            $"{icons["antenna"]} Market Cap: {marketCapStr} \n" +
                            $"{icons["chart"]} [dextools]({optionsTelegram.dextoolsUrl}app/en/ether/pair-explorer/{item.pairAddress}) " +
                            $"{icons["chart"]} [dexscreener]({optionsTelegram.dexscreenerUrl}ethereum/{item.pairAddress})";
                        break;

                    default:
                        item.messageText =
                            item.line_tokenName + " \n" +
                            item.line_tokenAddress + " \n" +

                            $"{icons["clockSend"]} {average.periodInMins} mins   \n" +
                            $"{icons["buy"]} Buy av  {(decimal)average.volumePositiveEthAverage:0.##} {item.currency}  {icons["sell"]} Sell av  {(decimal)average.volumeNegativeEthAverage:0.##} {item.currency} \n" +
                            $"{buyToSell} Now buy:  {(decimal)average.last.volumePositiveEth:0.##} {item.currency}  Sell:  {(decimal)average.last.volumeNegativeEth:0.##} {item.currency}  \n" +
                            $"{xxx} {x:0.##} X \n" +
                            $"{icons["calendar"]} {average.last.blockIntStartDate.ToShortTimeString()} / {average.last.blockIntEndDate.ToShortTimeString()} \n" +
                            $"{icons["antenna"]} Market Cap: {marketCapStr} \n" +
                            $"{icons["SCROLL"]} Count of triggers: {volumeRiseCount.Count} \n" +
                            $"{icons["chart"]} [dextools]({optionsTelegram.dextoolsUrl}app/en/ether/pair-explorer/{item.pairAddress}) " +
                            $"{icons["chart"]} [dexscreener]({optionsTelegram.dexscreenerUrl}ethereum/{item.pairAddress})";
                        break;
                }

            }

            foreach (var item in collection)
            {
                var t = await SendSequest(threadId, item.messageText, chat_id);
                item.tlgrmMsgId = t;
            }

            return collection;
        }
    }
}
