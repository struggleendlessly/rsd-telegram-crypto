using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;

using System.Net.Http;

namespace api_alchemy
{
    public class ApiAlchemy
    {
        private readonly HttpClient httpClient;
        private readonly OptionsAlchemy optionsAlchemy;

        public ApiAlchemy(
            IHttpClientFactory httpClient,
            IOptions<OptionsAlchemy> optionsAlchemy)
        {
            this.optionsAlchemy = optionsAlchemy.Value;
            var url = this.optionsAlchemy.UrlBase.Replace("{{{chainName}}}", this.optionsAlchemy.ChainNames.Etherium);

            this.httpClient = httpClient.CreateClient("ApiAlchemy");
            this.httpClient.BaseAddress = new Uri(url);
        }

        public async Task<string> getBlockByNumber(int block)
        {
            StringContent httpContent = new StringContent(EthUrlBuilder.getBlockByNumber(), System.Text.Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("/v2/3ex6KSnDnxa98_q_F6CD26ByLMK4Gga-", httpContent);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
