using api_alchemy.Eth.ResponseDTO;

using Data.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;

using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace api_alchemy.Eth
{
    public class EthApi
    {
        private readonly int batchSize = 50;
        private readonly int maxDiffToProcess = 1000;

        private readonly ILogger logger;
        private readonly HttpClient httpClient;
        private readonly OptionsAlchemy optionsAlchemy;
        private string vAndApiKey;
        public EthApi(
            ILogger<EthApi> logger,
            IHttpClientFactory httpClient,
            IOptions<OptionsAlchemy> _optionsAlchemy)
        {
            this.logger = logger;
            optionsAlchemy = _optionsAlchemy.Value;
            var url = optionsAlchemy.UrlBase.Replace("{{{chainName}}}", optionsAlchemy.ChainNames.Etherium);

            this.httpClient = httpClient.CreateClient("Api");
            this.httpClient.BaseAddress = new Uri(url);

            Random rnd = new Random();
            var apiKeyIndex = rnd.Next(0, optionsAlchemy.ApiKeys.Length - 1);
            vAndApiKey = $"/v2/{optionsAlchemy.ApiKeys[apiKeyIndex]}";
        }

        private string GetvAndApiKey(int index)
        {
            var key = optionsAlchemy.ApiKeys[index];
            var res = $"/v2/{key}";
            return res;
        }

        public async Task<List<Response>> executeBatchCall
            <ApiInput, Response>(

            List<ApiInput> items,
            Func<List<ApiInput>, int, Task<List<Response>>> apiMethod,
            int diff = 0
            )
        {
            ConcurrentBag<Response> res = new();
            var MaxDegreeOfParallelism = 4;
            var batchSizeLocal = batchSize;

            if (apiMethod.Method.Name.Equals("getAssetTransfers", StringComparison.InvariantCultureIgnoreCase))
            {
                batchSizeLocal = 1;
            }


            if (diff > 0)
            {
                if (diff > maxDiffToProcess)
                {
                    items = items.Take(maxDiffToProcess).ToList();
                }

                var rangeChunks = items.Chunk(batchSizeLocal).ToList();
                var rangeForBatchesWithApiKey = new Dictionary<int, int>();
                var apiKeyParallelIndex = 0;

                for (int i = 0; i < rangeChunks.Count(); i++)
                {
                    rangeForBatchesWithApiKey.Add(i, apiKeyParallelIndex);

                    if (apiKeyParallelIndex + 1 < MaxDegreeOfParallelism)
                    {
                        apiKeyParallelIndex++;
                    }
                    else
                    {
                        apiKeyParallelIndex = 0;
                    }
                }

                await Parallel.ForEachAsync(
                    rangeForBatchesWithApiKey,
                    new ParallelOptions { MaxDegreeOfParallelism = MaxDegreeOfParallelism, },
                    async (data, ct) =>
                    {
                        var iterator = data.Key;
                        var apiKeyIndex = data.Value;

                        var chunk = rangeChunks[iterator].ToList();

                        var t = await apiMethod(chunk, apiKeyIndex);

                        foreach (var item in t)
                        {
                            res.Add(item);
                        }
                    });
            }

            return res.ToList();
        }

        // https://sandbox.alchemy.com/?network=ETH_MAINNET&method=eth_getBlockByNumber&body.id=1&body.jsonrpc=2.0&body.method=eth_getBlockByNumber&body.params%5B0%5D=finalized&body.params%5B1%5D=true
        // https://docs.alchemy.com/reference/eth-getblockbynumber
        public async Task<int> lastBlockNumber()
        {
            var res = 0;

            var body = EthUrlBuilder.lastBlockNumber();

            StringContent httpContent = new StringContent(
                body,
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(vAndApiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var t = await response.Content.ReadFromJsonAsync<lastBlockNumber>();

                if (t is not null)
                {
                    res = Convert.ToInt32(t.result, 16); ;
                }
            }

            return res;
        }

        public async Task<getBlockByNumberDTO> getBlockByNumber(
            int block)
        {
            getBlockByNumberDTO res = new getBlockByNumberDTO();

            var body = EthUrlBuilder.getBlockByNumber(block);

            StringContent httpContent = new StringContent(
                body,
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(vAndApiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var t = await response.Content.ReadFromJsonAsync<getBlockByNumberDTO>();

                if (t is not null)
                {
                    res = t;
                }
            }

            return res;
        }

        public async Task<List<getBlockByNumberDTO>> getBlockByNumberBatch(
            List<int> blocks,
            int apiKeyIndex)
        {
            var apiKey = GetvAndApiKey(apiKeyIndex);
            List<getBlockByNumberDTO> res = new();
            StringBuilder aa = new();

            aa.Append("[");

            foreach (var item in blocks)
            {
                aa.Append(EthUrlBuilder.getBlockByNumber(item));

                if (blocks.Last() != item)
                {
                    aa.Append(",");
                }
            }

            aa.Append("]");

            StringContent httpContent = new StringContent(
                aa.ToString(),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(apiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var t = await response.Content.ReadFromJsonAsync<List<getBlockByNumberDTO>>();

                if (t is not null)
                {
                    res = t;
                }
            }

            return res;
        }

        public async Task<List<getTransactionReceiptDTO>> getTransactionReceiptBatch(
            List<string> transactinHash,
            int apiKeyIndex)
        {
            var apiKey = GetvAndApiKey(apiKeyIndex);
            List<getTransactionReceiptDTO> res = new();
            StringBuilder aa = new();

            aa.Append("[");

            foreach (var item in transactinHash)
            {
                aa.Append(EthUrlBuilder.getTransactionReceipt(item));

                if (transactinHash.Last() != item)
                {
                    aa.Append(",");
                }
            }

            aa.Append("]");

            StringContent httpContent = new StringContent(
                aa.ToString(),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(apiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var t = await response.Content.ReadFromJsonAsync<List<getTransactionReceiptDTO>>();

                if (t is not null)
                {
                    res = t;
                }
            }

            return res;
        }

        public async Task<List<getTokenMetadataDTO>> getTokenMetadataBatch(
            List<getTransactionReceiptDTO.Result> txnReceipts,
            int apiKeyIndex)
        {
            var apiKey = GetvAndApiKey(apiKeyIndex);
            List<getTokenMetadataDTO> res = new();
            StringBuilder aa = new();

            aa.Append("[");

            foreach (var item in txnReceipts)
            {
                aa.Append(EthUrlBuilder.getTokenMetadata(
                    item.contractAddress,
                    item.txnNumberForMetadata));

                if (txnReceipts.Last() != item)
                {
                    aa.Append(",");
                }
            }

            aa.Append("]");

            StringContent httpContent = new StringContent(
                aa.ToString(),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(apiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var t = await response.Content.ReadFromJsonAsync<List<getTokenMetadataDTO>>();

                if (t is not null)
                {
                    res = t;
                }
            }

            return res;
        }

        public async Task<List<getBalance>> getBalance(
            List<EthTrainData> EthTrainData,
            int apiKeyIndex)
        {
            var apiKey = GetvAndApiKey(apiKeyIndex);
            List<getBalance> res = new();
            StringBuilder aa = new();

            aa.Append("[");

            foreach (var item in EthTrainData)
            {
                aa.Append(EthUrlBuilder.getBalance(
                    item.from,
                    item.Id,
                    item.blockNumber));

                if (EthTrainData.Last() != item)
                {
                    aa.Append(",");
                }
            }

            aa.Append("]");

            StringContent httpContent = new StringContent(
                aa.ToString(),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(apiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var t = await response.Content.ReadFromJsonAsync<List<getBalance>>();

                if (t is not null)
                {
                    res = t;
                }
            }

            return res;
        }

        public async Task<List<getTotalSupplyDTO>> getTotalSupplyBatch(
            List<getTransactionReceiptDTO.Result> txnReceipts,
            int apiKeyIndex)
        {
            var apiKey = GetvAndApiKey(apiKeyIndex);
            var totalSupplyMethodCode = "0x18160ddd";
            List<getTotalSupplyDTO> res = new();
            StringBuilder aa = new();

            aa.Append("[");

            foreach (var item in txnReceipts)
            {
                aa.Append(EthUrlBuilder.eth_call(
                    item.contractAddress,
                    item.txnNumberForMetadata,
                    totalSupplyMethodCode));

                if (txnReceipts.Last() != item)
                {
                    aa.Append(",");
                }
            }

            aa.Append("]");

            StringContent httpContent = new StringContent(
                aa.ToString(),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(apiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var t = await response.Content.ReadFromJsonAsync<List<getTotalSupplyDTO>>();

                if (t is not null)
                {
                    res = t;
                }
            }

            return res;
        }

        public async Task<List<getAssetTransfersDTO>> getAssetTransfers(
            List<EthTrainData> EthTrainData,
            int apiKeyIndex)
        {
            var apiKey = GetvAndApiKey(apiKeyIndex);
            var item = EthTrainData.First();

            List<getAssetTransfersDTO> res = new();
            var data = EthUrlBuilder.alchemy_getAssetTransfers(
                                item.from,
                                item.blockNumber);

            StringContent httpContent = new StringContent(
                data,
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(apiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var t = await response.Content.ReadFromJsonAsync<getAssetTransfersDTO>();

                if (t is not null)
                {
                    res.Add(t);
                }
            }

            return res;
        }
    }
}
