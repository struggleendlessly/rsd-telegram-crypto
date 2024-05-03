using Microsoft.Extensions.Options;

using Shared.BaseScan.Model;
using Shared.ConfigurationOptions;

using System.Net.Http.Json;
using System.Text;

namespace Shared.BaseScan
{
    public class BaseScan
    {
        private readonly OptionsBaseScan optionsBaseScan;
        public BaseScan(IOptions<OptionsBaseScan> optionsBaseScan)
        {
            this.optionsBaseScan = optionsBaseScan.Value;
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
            urlBuilder.Append($"&apikey={optionsBaseScan.apiKeyToken}");

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
            urlBuilder.Append($"&apikey={optionsBaseScan.apiKeyToken}");

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
            urlBuilder.Append($"&apikey={optionsBaseScan.apiKeyToken}");

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
