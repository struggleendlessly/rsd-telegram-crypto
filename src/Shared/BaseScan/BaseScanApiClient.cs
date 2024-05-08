using Azure;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

using Shared.BaseScan.Model;
using Shared.ConfigurationOptions;

using System;
using System.Net.Http.Json;
using System.Text;

namespace Shared.BaseScan
{
    public class BaseScanApiClient
    {
        private readonly OptionsBaseScan optionsBaseScan;
        private string apiKeyToken;

        public BaseScanApiClient(IOptions<OptionsBaseScan> optionsBaseScan)
        {
            this.optionsBaseScan = optionsBaseScan.Value;
            SetApiKeyToken();
        }
        public void SetApiKeyToken(int apiKeyTokenNumber = 0)
        {
            switch (apiKeyTokenNumber)
            {
                case 1:
                    apiKeyToken = optionsBaseScan.apiKeyToken1;
                    break;

                default:
                    apiKeyToken = optionsBaseScan.apiKeyToken;
                    break;
            }
        }
        public async Task<AddressModel> GetInfoByAddress(string address)
        {
            AddressModel res = new();

            var url = await UrlBuilderAddress(address);

            using (HttpClient sharedClient = new() { BaseAddress = new Uri(optionsBaseScan.baseUrl) })
            {
                HttpResponseMessage response = await sharedClient.GetAsync(url);

                response.EnsureSuccessStatusCode().WriteRequestToConsole();

                res = await response.Content.ReadFromJsonAsync<AddressModel>();
            }

            return res;
        }

        private async Task<string> UrlBuilderAddress(string address)
        {
            var res = string.Empty;

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append("api/?module=account");
            urlBuilder.Append("&action=txlist");
            urlBuilder.Append($"&address={address}");
            urlBuilder.Append("&startblock=0");
            urlBuilder.Append("&endblock=99999999");
            urlBuilder.Append("&page=1");
            urlBuilder.Append("&offset=1000");
            urlBuilder.Append("&sort=asc");
            urlBuilder.Append($"&apikey={apiKeyToken}");

            res = urlBuilder.ToString();

            return res;
        }

        public async Task<TotalSupplyModel> GetTotalSupply(string address)
        {
            TotalSupplyModel res = new();

            var url = await UrlBuilderTotalSupply(address);

            using (HttpClient sharedClient = new() { BaseAddress = new Uri(optionsBaseScan.baseUrl) })
            {
                HttpResponseMessage response = await sharedClient.GetAsync(url);

                response.EnsureSuccessStatusCode().WriteRequestToConsole();

                res = await response.Content.ReadFromJsonAsync<TotalSupplyModel>();
            }

            return res;
        }

        private async Task<string> UrlBuilderTotalSupply(string address)
        {
            var res = string.Empty;

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append("api/?module=stats");
            urlBuilder.Append("&action=tokensupply");
            urlBuilder.Append($"&contractaddress={address}");
            urlBuilder.Append($"&apikey={apiKeyToken}");

            res = urlBuilder.ToString();

            return res;
        }
        public async Task<ContractSourceCodeModel> GetContractSourceCode(string address)
        {
            ContractSourceCodeModel res = new();

            var url = await UrlBuilderContractSourceCode(address);

            using (HttpClient sharedClient = new() { BaseAddress = new Uri(optionsBaseScan.baseUrl) })
            {
                HttpResponseMessage response = await sharedClient.GetAsync(url);

                response.EnsureSuccessStatusCode().WriteRequestToConsole();

                res = await response.Content.ReadFromJsonAsync<ContractSourceCodeModel>();
            }

            return res;
        }

        private async Task<string> UrlBuilderContractSourceCode(string address)
        {
            var res = string.Empty;

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append("api/?module=contract");
            urlBuilder.Append("&action=getsourcecode");
            urlBuilder.Append($"&address={address}");
            urlBuilder.Append($"&apikey={apiKeyToken}");

            res = urlBuilder.ToString();

            return res;
        }

        public async Task<BlockByNumberModel> GetBlockByNumber(string blockNumbderX16)
        {
            BlockByNumberModel res = new();

            var url = await UrlBuilderGetBlockByNumber(blockNumbderX16);

            using (HttpClient sharedClient = new() { BaseAddress = new Uri(optionsBaseScan.baseUrl) })
            {
                HttpResponseMessage response = await sharedClient.GetAsync(url);

                response.EnsureSuccessStatusCode().WriteRequestToConsole();

                res = await response.Content.ReadFromJsonAsync<BlockByNumberModel>();
            }

            return res;
        }

        private async Task<string> UrlBuilderGetBlockByNumber(string blockNumbderX16)
        {
            var res = string.Empty;

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append("api/?module=proxy");
            urlBuilder.Append("&action=eth_getBlockByNumber");
            urlBuilder.Append($"&tag=0x{blockNumbderX16}");
            urlBuilder.Append($"&boolean=true");
            urlBuilder.Append($"&apikey={apiKeyToken}");

            res = urlBuilder.ToString();

            return res;
        }

        public async Task<LastBlockNumberModel> GetLastBlockByNumber()
        {
            LastBlockNumberModel res = new();

            var url = await UrlBuilderGetLastBlockByNumber();

            using (HttpClient sharedClient = new() { BaseAddress = new Uri(optionsBaseScan.baseUrl) })
            {
                HttpResponseMessage response = await sharedClient.GetAsync(url);

                response.EnsureSuccessStatusCode().WriteRequestToConsole();

                res = await response.Content.ReadFromJsonAsync<LastBlockNumberModel>();
            }

            return res;
        }

        private async Task<string> UrlBuilderGetLastBlockByNumber()
        {
            var res = string.Empty;

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append("api/?module=proxy");
            urlBuilder.Append("&action=eth_blockNumber");
            urlBuilder.Append($"&apikey={apiKeyToken}");

            res = urlBuilder.ToString();

            return res;
        }

        public async Task<NormalTransactions> GetListOfNormalTransactions(string ownerAddress, int page = 1)
        {
            NormalTransactions res = new();

            var url = await UrlBuilderGetListOfNormalTransactions(ownerAddress, page);

            using (HttpClient sharedClient = new() { BaseAddress = new Uri(optionsBaseScan.baseUrl) })
            {
                HttpResponseMessage response = await sharedClient.GetAsync(url);

                response.EnsureSuccessStatusCode().WriteRequestToConsole();

                res = await response.Content.ReadFromJsonAsync<NormalTransactions>();
            }

            return res;
        }

        private async Task<string> UrlBuilderGetListOfNormalTransactions(string ownerAddress, int page = 1)
        {
            var res = string.Empty;

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append("api/?module=account");
            urlBuilder.Append("&action=txlist");
            urlBuilder.Append($"&address={ownerAddress}");
            urlBuilder.Append($"&startblock=0");
            urlBuilder.Append($"&endblock=99999999");
            urlBuilder.Append($"&startblock=0");
            urlBuilder.Append($"&page={page}");
            urlBuilder.Append("&offset=1500");
            urlBuilder.Append("&sort=asc");
            urlBuilder.Append($"&apikey={apiKeyToken}");

            res = urlBuilder.ToString();

            return res;
        }
    }
}

static class HttpResponseMessageExtensions
{
    internal static void WriteRequestToConsole(this HttpResponseMessage response)
    {
        if (response is null)
        {
            return;
        }

        var request = response.RequestMessage;
        Console.Write($"{request?.Method} ");
        Console.Write($"{request?.RequestUri} ");
        Console.WriteLine($"HTTP/{request?.Version}");
    }
}
