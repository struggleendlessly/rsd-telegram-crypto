using Azure;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;

using Polly;

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

        private readonly IAsyncPolicy<HttpResponseMessage> retryPolicy =
            Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode is >= System.Net.HttpStatusCode.InternalServerError)
            .Or<Exception>()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

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

                case 2:
                    apiKeyToken = optionsBaseScan.apiKeyToken2;
                    break;

                default:
                    apiKeyToken = optionsBaseScan.apiKeyToken;
                    break;
            }
        }
        private async Task<T> RequestApi<T>(string address)
            where T : new()
        {
            T res = new();

            var url = address;
            int count = 0;

        repeat:
            using (HttpClient sharedClient = new() { BaseAddress = new Uri(optionsBaseScan.baseUrl) })
            {
                sharedClient.Timeout = TimeSpan.FromSeconds(30);
                HttpResponseMessage response = await retryPolicy.ExecuteAsync(() => sharedClient.GetAsync(url));

                response.EnsureSuccessStatusCode().WriteRequestToConsole();

                try
                {
                    res = await response.Content.ReadFromJsonAsync<T>();
                }
                catch (Exception ex)
                {
                    if (count < 3)
                    {
                        count++;
                        await Task.Delay(1000);
                        goto repeat;
                    }
                    else
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        Console.WriteLine(json);
                        throw;
                    }
                }
            }

            return res;
        }

        public async Task<AddressModel> GetInfoByAddress(string address)
        {
            AddressModel res = new();

            var url = await UrlBuilderAddress(address);

            res = await RequestApi<AddressModel>(url);

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

            res = await RequestApi<TotalSupplyModel>(url);

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

            res = await RequestApi<ContractSourceCodeModel>(url);
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

            res = await RequestApi<BlockByNumberModel>(url);

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

            res = await RequestApi<LastBlockNumberModel>(url);

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

            res = await RequestApi<NormalTransactions>(url);

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
            urlBuilder.Append("&offset=999");
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
