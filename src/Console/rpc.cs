using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

public class JsonRpcClient
{
    //https://base-mainnet.g.alchemy.com/v2/auVywuqgowX5rF1_h3CODjaIO6Lj32o9
    //auVywuqgowX5rF1_h3CODjaIO6Lj32o9

    private readonly HttpClient httpClient;
    private int requestId = 1;

    public JsonRpcClient(string baseUrl)
    {
        httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(baseUrl);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> SendRequest()
    {
        var jsonRequest =
        """
            {
              "id": 1,
              "jsonrpc": "2.0",
              "method": "eth_call",
              "params": [
                {
                  "to": "0xc45a81bc23a64ea556ab4cdf08a86b61cdceea8b",
                  "data": "0x0902f1ac"
                },
                "0x139545E"
              ]
            }
            """;

        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("v2/3ex6KSnDnxa98_q_F6CD26ByLMK4Gga-", content);
        var t = await response.Content.ReadAsStringAsync();

        return t;
    }
}

