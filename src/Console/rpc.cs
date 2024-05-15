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

    public async Task<TResponse> SendRequest<TRequest, TResponse>(string method, TRequest request)
    {
        var jsonRequest = new
        {
            jsonrpc = "2.0",
            method = method,
            @params = """
            "params": [{"address":"0x4200000000000000000000000000000000000006","fromBlock":"0xBC5C7B","toBlock":"0xBC5C7D","topics":["0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef"]}]
            """,
            id = requestId++
        };
        var tt = JsonSerializer.Serialize(jsonRequest);
        var content = new StringContent(JsonSerializer.Serialize(jsonRequest), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("", content);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Request failed with status code {response.StatusCode}");
        }

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var rpcResponse = JsonSerializer.Deserialize<JsonRpcResponse<TResponse>>(jsonResponse, jsonOptions);

        if (rpcResponse.Error != null)
        {
            throw new Exception($"Request failed with error: {rpcResponse.Error.Message}");
        }

        return rpcResponse.Result;
    }
}

public class Rootobject
{
    public Class1[] Property1 { get; set; }
}

public class Class1
{
    public string address { get; set; }
    public string fromBlock { get; set; }
    public string toBlock { get; set; }
    public string[] topics { get; set; }
}


public class JsonRpcResponse<T>
{
    public T Result { get; set; }
    public JsonRpcError Error { get; set; }
}

public class JsonRpcError
{
    public int Code { get; set; }
    public string Message { get; set; }
}

// Usage example
public class Program1
{
    public static async Task Main()
    {
        var rpcClient = new JsonRpcClient("https://base-mainnet.g.alchemy.com/v2/auVywuqgowX5rF1_h3CODjaIO6Lj32o9");

        var params1 = """[{"address":"0x4200000000000000000000000000000000000006","fromBlock":"0xBC5C7B","toBlock":"0xBC5C7D","topics":["0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef"]}]""";
        var e = new Rootobject();
        e.Property1 = new Class1[1];

        var c = new Class1();
        c.address = "0x4200000000000000000000000000000000000006";
        c.fromBlock = "0xBC5C7B";
        c.toBlock = "0xBC5C7D";
        c.topics = new string[1] ;
        c.topics[0] = "0xddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";
        var request = new
        {

            // Request parameters
        };

        try
        {
            var response = await rpcClient.SendRequest<object, object>("eth_getLogs", c);
            Console.WriteLine(JsonSerializer.Serialize(params1));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
