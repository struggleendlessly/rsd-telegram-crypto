using etherscan.ResponseDTO;

using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Http.Json;

namespace etherscan
{
    public class EtherscanApi
    {

        private readonly HttpClient httpClient;
        private readonly OptionsEtherscan optionsEtherscan;
        private readonly int MaxDegreeOfParallelism = 4;

        public EtherscanApi(
            IHttpClientFactory httpClient,
            IOptions<OptionsEtherscan> _optionsEtherscan)
        {
            optionsEtherscan = _optionsEtherscan.Value;

            var url = optionsEtherscan.UrlBase;

            this.httpClient = httpClient.CreateClient("Api");
            this.httpClient.BaseAddress = new Uri(url);
        }

        private string GetApiKey(int index)
        {
            var res = optionsEtherscan.ApiKeys[index];
            return res;
        }

        public async Task<List<GetSourceCodeDTO>> getSourceCodeBatchRequest(List<string> contractAddresses)
        {
            ConcurrentBag<GetSourceCodeDTO> res = new();

            var requests = getSourceCodeBatch(contractAddresses);
            var chunks = requests.Chunk(MaxDegreeOfParallelism).ToList();

            await Parallel.ForEachAsync(
                chunks,
                new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism, },
                async (data, ct) =>
                {
                    var response = await httpClient.GetAsync(data);

                    if (response.IsSuccessStatusCode)
                    {
                        var t = await response.Content.ReadFromJsonAsync<getBlockByNumberDTO>();

                        if (t is not null)
                        {
                            res = t;
                        }
                    }

                    foreach (var item in t)
                    {
                        res.Add(item);
                    }
                });

            return res.ToList();
        }

        private List<string> getSourceCodeBatch(List<string> contractAddresses)
        {
            List<string> res = new();

            var apiKeyParallelIndex = 0;

            for (int i = 0; i < contractAddresses.Count; i++)
            {
                var apiKey = GetApiKey(apiKeyParallelIndex);
                var url = UrlBuider.getSourceCode(contractAddresses[i], apiKey);
                res.Add(url);

                if (apiKeyParallelIndex + 1 < MaxDegreeOfParallelism)
                {
                    apiKeyParallelIndex++;
                }
                else
                {
                    apiKeyParallelIndex = 0;
                }
            }

            return res;
        }

    }
}
