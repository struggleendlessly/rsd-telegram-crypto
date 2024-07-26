using System.Text.Json.Serialization;

namespace api_alchemy.Eth.RequestDTO
{

    public class requestBaseDTO
    {
        public string jsonrpc { get; set; } = string.Empty;
        public string method { get; set; } = string.Empty;

        [JsonPropertyName("params")]
        public object[]? _params { get; set; } = null;
        public int id { get; set; } = 0;
    }

}
