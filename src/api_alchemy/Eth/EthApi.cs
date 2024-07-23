using api_alchemy.Eth.ResponseDTO;
using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;

using System.Net.Http;
using System.Net.Http.Json;

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
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(vAndApiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                var t = await response.Content.ReadFromJsonAsync<lastBlockNumber>();
                res = Convert.ToInt32(t.result, 16);
            }

            return res;
        }    
        
        public async Task<getBlockByNumberDTO> getBlockByNumber(int block)
        {
            getBlockByNumberDTO res = new getBlockByNumberDTO();

            var body = EthUrlBuilder.getBlockByNumber(block);

            StringContent httpContent = new StringContent(
                body,
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync(vAndApiKey, httpContent);

            if (response.IsSuccessStatusCode)
            {
                res = await response.Content.ReadFromJsonAsync<getBlockByNumberDTO>();
            }

            return res;
        }
    }
}
