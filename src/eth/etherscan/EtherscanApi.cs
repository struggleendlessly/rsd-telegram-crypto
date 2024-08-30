using etherscan.ResponseDTO;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;

using System.Collections.Concurrent;
using System.Net.Http.Json;

namespace etherscan
{
    public class EtherscanApi
    {

        private readonly ILogger _logger;
        private readonly HttpClient httpClient;
        private readonly OptionsEtherscan optionsEtherscan;

        private readonly int MaxDegreeOfParallelism = 4;
        private readonly int apiKeysCount = 0;

        public EtherscanApi(
            ILogger<EtherscanApi> logger,
            IHttpClientFactory httpClient,
            IOptions<OptionsEtherscan> _optionsEtherscan)
        {
            _logger = logger;
            optionsEtherscan = _optionsEtherscan.Value;

            var url = optionsEtherscan.UrlBase;

            this.httpClient = httpClient.CreateClient("Api");
            this.httpClient.BaseAddress = new Uri(url);

            apiKeysCount = optionsEtherscan.ApiKeys.Count();
        }

        private string GetApiKey(int index)
        {
            var res = optionsEtherscan.ApiKeys[index];
            return res;
        }

        public async Task<List<GetSourceCodeDTO>> getSourceCodeBatchRequest(
            List<string> contractAddresses)
        {
            ConcurrentBag<GetSourceCodeDTO> res = new();

            var requests = getSourceCodeBatch(contractAddresses);
            var chunks = requests.Chunk(MaxDegreeOfParallelism).ToList();

            await Parallel.ForEachAsync(
                requests,
                new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism, },
                async (data, ct) =>
                {
                    var response = await httpClient.GetAsync(data.Key);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseText = string.Empty;
                        try
                        {

                            responseText = await response.Content.ReadAsStringAsync();
                            var t = await response.Content.ReadFromJsonAsync<GetSourceCodeDTO>();
                            if (true)
                            {

                            }
                            if (t is not null &&
                                t.result is not null)
                            {
                                t.contractAddress = data.Value;
                                res.Add(t);
                            }

                            Thread.Sleep(100);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("getSourceCodeBatchRequest error, response: {response}", responseText);
                            throw;
                        }
                    }

                    Thread.Sleep(100);
                });

            return res.ToList();
        }

        public async Task<List<GetNormalTxnDTO>> getNormalTxnBatchRequest(
           List<(string from, string blockNumber)> ownerAddresses)
        {
            ConcurrentBag<GetNormalTxnDTO> res = new();

            var requests = getNormalTxnBatch(ownerAddresses);
            var chunks = requests.Chunk(MaxDegreeOfParallelism).ToList();

            await Parallel.ForEachAsync(
                requests,
                new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism, },
                async (data, ct) =>
                {

                    var response = await httpClient.GetAsync(data.Key);

                    if (response.IsSuccessStatusCode)
                    {
                        var ee = string.Empty;
                        try
                        {

                            ee = await response.Content.ReadAsStringAsync();
                            var t = await response.Content.ReadFromJsonAsync<GetNormalTxnDTO>();

                            if (t is not null &&
                                t.result is not null)
                            {
                                t.ownerAddresses = data.Value;
                                res.Add(t);
                            }
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    }

                    Thread.Sleep(100);
                });

            return res.ToList();
        }

        private Dictionary<string, string> getSourceCodeBatch
            (List<string> contractAddresses)
        {
            Dictionary<string, string> res = new();

            var currentHour = DateTime.Now.Hour % 2;
            var apiKeyParallelIndex = 0;

            if (currentHour != 0)
            {
                apiKeyParallelIndex = apiKeysCount - 1;
            }

            for (int i = 0; i < contractAddresses.Count; i++)
            {
                var item = contractAddresses[i];
                var apiKey = GetApiKey(apiKeyParallelIndex);
                var url = UrlBuider.getSourceCode(item, apiKey);
                res.Add(url, item);

                if (currentHour == 0)
                {
                    if (apiKeyParallelIndex + 1 < apiKeysCount)
                    {
                        apiKeyParallelIndex++;
                    }
                    else
                    {
                        apiKeyParallelIndex = 0;
                    }
                }
                else
                {
                    if (apiKeyParallelIndex - 1 >= 0)
                    {
                        apiKeyParallelIndex--;
                    }
                    else
                    {
                        apiKeyParallelIndex = apiKeysCount - 1;
                    }
                }
            }

            return res;
        }

        private Dictionary<string, string> getNormalTxnBatch
            (List<(string from, string blockNumber)> ownerAddresses)
        {
            Dictionary<string, string> res = new();

            var currentHour = DateTime.Now.Hour % 2;
            var apiKeyParallelIndex = 0;

            if (currentHour != 0)
            {
                apiKeyParallelIndex = apiKeysCount - 1;
            }

            for (int i = 0; i < ownerAddresses.Count; i++)
            {
                var item = ownerAddresses[i];
                var apiKey = GetApiKey(apiKeyParallelIndex);
                var url = UrlBuider.getNormalTxn(item, apiKey);
                res.Add(url, item.from);

                Console.WriteLine(item);

                if (currentHour == 0)
                {
                    if (apiKeyParallelIndex + 1 < apiKeysCount)
                    {
                        apiKeyParallelIndex++;
                    }
                    else
                    {
                        apiKeyParallelIndex = 0;
                    }
                }
                else
                {
                    if (apiKeyParallelIndex - 1 >= 0)
                    {
                        apiKeyParallelIndex--;
                    }
                    else
                    {
                        apiKeyParallelIndex = apiKeysCount - 1;
                    }
                }
            }

            return res;
        }

    }
}
