using Data.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;
using Shared.Telegram.Models;

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

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

        public async Task<int> SendPO(
            List<EthTrainData> ethTrainDatas,
            List<EthBlocks> ethBlocks)
        {
            P0_DTO val = new();

            foreach (var item in ethTrainDatas)
            {
                var block = ethBlocks.FirstOrDefault(x => x.numberInt == item.blockNumberInt);
                int intUnix = Convert.ToInt32(block.timestamp, 16);
                DateTime dateTimeBlock = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                dateTimeBlock = dateTimeBlock.AddSeconds(intUnix);

                val.walletAge = (-1).ToString();

                if (block == null)
                {
                    var age = dateTimeBlock - item.walletCreated;
                    val.walletAge = age.Days.ToString();
                }

                val.from = item.from;
                val.totalSupply = item.totalSupply;
                val.ABI = item.ABI;
                val.contractAddress = item.contractAddress;
                val.balanceOnCreating = item.BalanceOnCreating.ToString();
                val.name = item.name;
                val.symbol = item.symbol;
            }


            var res = 0;
            var threadId = optionsTelegram.message_thread_id_p0;
            var isABI = "❤";
            if (!string.IsNullOrEmpty(val.ABI))
            {
                isABI = "💚";
            }

            var text =
                $"" +
                $"📌 [{val.name}({val.symbol})]({optionsTelegram.etherscanUrl}token/{val.contractAddress}) \n" +
                $"{isABI}`{val.contractAddress}` \n " +
                $"😉`{val.totalSupply}` \n " +
                $"[Deployer]({optionsTelegram.etherscanUrl}address/{val.from}) / {val.walletAge} / {val.balanceOnCreating}  \n" +
                $"";


            string urlString = $"bot{optionsTelegram.bot_hash}/" +
            $"sendMessage?" +
                $"message_thread_id={threadId}&" +
                $"chat_id={optionsTelegram.chat_id_coins}&" +
                $"text={text}&" +
                $"parse_mode=MarkDown&" +
                $"disable_web_page_preview=true";
            try
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                var response = await httpClient.GetFromJsonAsync<MessageSend>(urlString);


                await Task.Delay(optionsTelegram.api_delay_forech);
            }
            catch (Exception t)
            {

                throw;
            }



            return res;
        }
    }
}
