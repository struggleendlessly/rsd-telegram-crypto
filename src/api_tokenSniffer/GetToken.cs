

using Data.Models;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Collections.Concurrent;
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

    }
}
