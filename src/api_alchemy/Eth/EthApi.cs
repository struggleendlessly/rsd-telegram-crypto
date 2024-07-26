using api_alchemy.Eth.ResponseDTO;

using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;

using System.Net.Http.Json;
using System.Text;

namespace api_alchemy.Eth
{
    public class EthApi
    {
        private readonly HttpClient httpClient;
        private readonly OptionsAlchemy optionsAlchemy;
        private string vAndApiKey = string.Empty;
        public EthApi(
            IHttpClientFactory httpClient,
            IOptions<OptionsAlchemy> _optionsAlchemy)
        {
            optionsAlchemy = _optionsAlchemy.Value;
            var url = optionsAlchemy.UrlBase.Replace("{{{chainName}}}", optionsAlchemy.ChainNames.Etherium);

            this.httpClient = httpClient.CreateClient("ApiAlchemy");
            this.httpClient.BaseAddress = new Uri(url);

            Random rnd = new Random();
            var apiKeyIndex = rnd.Next(0, optionsAlchemy.ApiKeys.Length - 1);
            vAndApiKey = $"/v2/{optionsAlchemy.ApiKeys[apiKeyIndex]}";
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
            List<int> blocks)
        {
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

            var response = await httpClient.PostAsync(vAndApiKey, httpContent);

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
            List<string> transactinHash)
        {
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

            var response = await httpClient.PostAsync(vAndApiKey, httpContent);

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
            List<string> transactinHash)
        {
            List<getTokenMetadataDTO> res = new();
            StringBuilder aa = new();

            aa.Append("[");

            foreach (var item in transactinHash)
            {
                aa.Append(EthUrlBuilder.getTokenMetadata(item));

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

            var response = await httpClient.PostAsync(vAndApiKey, httpContent);

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
    }
}
