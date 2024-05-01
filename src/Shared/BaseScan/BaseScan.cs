using Shared.BaseScan.Model;

using System.Net.Http.Json;
using System.Text;

namespace Shared.BaseScan
{
    public class BaseScan
    {
        string baseUrl = "https://api.basescan.org";
        string apiKeyToken = "VPD99DPE6Z57DP1QNYRXAUS4PHR54B1QZW";
        public BaseScan()
        {

        }

        public async Task<AddressModel> GetInfoByAddress(string address)
        {
            AddressModel res = new();

            var url = await UrlBuilderAddress(address);

            using (HttpClient sharedClient = new() { BaseAddress = new Uri(baseUrl) })
            {
                HttpResponseMessage response = await sharedClient.GetAsync(url);

                response.EnsureSuccessStatusCode().WriteRequestToConsole();

                res = await response.Content.ReadFromJsonAsync<AddressModel>();
            }

            return res;
        }

        public async Task<string> UrlBuilderAddress(string address)
        {
            var res = string.Empty;

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append("api/?module=account");
            urlBuilder.Append("&action=txlist");
            urlBuilder.Append($"&address={address}");
            urlBuilder.Append("&startblock=0");
            urlBuilder.Append("&endblock=99999999");
            urlBuilder.Append("&page=1");
            urlBuilder.Append("&offset=100");
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
