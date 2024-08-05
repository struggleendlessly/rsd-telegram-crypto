

using Data.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


namespace api_tokenSniffer
{
    public class GetToken
    {
        private readonly ILogger logger;
        private readonly HttpClient httpClient;

        public GetToken(
            ILogger<GetToken> logger,
            IHttpClientFactory httpClient
            )
        {
            this.logger = logger;

            var url = "https://tokensniffer.com";
            this.httpClient = httpClient.CreateClient("Api");
            this.httpClient.BaseAddress = new Uri(url);
        }

        public async Task Start()
        {
            //var res = await Get();

            //return res;
        }

        public async Task<GetTokenDTO> Get(string tokenAddress)
        {
            GetTokenDTO res = new();

            var apiKey = "db6457b1da76d753cdd3ab22bdec40d29523ac9f";
            var url = $"api/v2/tokens/1/{tokenAddress}?apikey={apiKey}&include_metrics=true&include_tests=true&include_similar=true&block_until_ready=true";

            var response = await httpClient.GetFromJsonAsync<GetTokenDTO>(url);

            if (response is not null)
            {
                res = response;
            }

            return res;
        }
    }
}
