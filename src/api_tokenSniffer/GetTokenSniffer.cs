

using Data;
using Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


namespace api_tokenSniffer
{
    public class GetTokenSniffer
    {
        private readonly ILogger logger;
        private readonly dbContext dbContext;
        private readonly HttpClient httpClient;

        public GetTokenSniffer(
            dbContext dbContext,
            ILogger<GetTokenSniffer> logger,
            IHttpClientFactory httpClient
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;

            var url = "https://tokensniffer.com";
            this.httpClient = httpClient.CreateClient("Api");
            this.httpClient.BaseAddress = new Uri(url);
        }

        public async Task Start()
        {
            var tokensToProcess = await GetTokensToProcess();
            var unfiltered = await Get(tokensToProcess);
            var processed = await Process(tokensToProcess, unfiltered);
            var res = await SaveToDB_update(processed);
        }

        public async Task<List<EthTrainData>> Process(
            List<EthTrainData> ethTrainDatas,
            List<GetTokenDTO> getTokenDTOs)
        {
            var res = new List<EthTrainData>();

            foreach (var item in ethTrainDatas)
            {
                var t = getTokenDTOs.Where(x => x.address == item.contractAddress).FirstOrDefault();

                if (t is not null)
                {
                    item.tsExploits = string.Join(",", t.exploits);
                    item.tsFullResponse = JsonSerializer.Serialize(t);

                }
                else
                {
                    item.tsFullResponse = "no";
                }

                res.Add(item);
            }

            return res;
        }

        private async Task<int> SaveToDB_update
             (List<EthTrainData> ethTrainDatas)
        {
            var res = 0;

            //dbContext.EthTrainData.UpdateRange(ethTrainDatas);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        public async Task<List<EthTrainData>> GetTokensToProcess()
        {
            var res = await
                dbContext.
                EthTrainData.
                Where(
                    x =>
                    x.pairAddress != "no" &&
                    x.tsFullResponse == "" &&
                    x.tsFullResponse != "no"
                    ).
                Take(410).
                ToListAsync();

            return res;
        }

        public async Task<List<GetTokenDTO>> Get(List<EthTrainData> ethTrainDatas)
        {
            List<GetTokenDTO> res = new();

            var apiKey = "db6457b1da76d753cdd3ab22bdec40d29523ac9f";

            foreach (var item in ethTrainDatas)
            {
                var tokenAddress = item.contractAddress;
                var url = $"api/v2/tokens/1/{tokenAddress}?apikey={apiKey}&include_metrics=true&include_tests=true&include_similar=true&block_until_ready=true";

                try
                {
                    var response = await httpClient.GetFromJsonAsync<GetTokenDTO>(url);

                    if (response is not null)
                    {
                        res.Add(response);
                    }
                }
                catch (Exception ex)
                {
                    continue;
                }

                Thread.Sleep(100);
            }

            return res;
        }
    }
}
