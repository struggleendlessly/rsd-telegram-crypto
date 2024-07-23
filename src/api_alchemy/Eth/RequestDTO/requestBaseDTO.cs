using System.Text.Json.Serialization;

namespace api_alchemy.Eth.RequestDTO
{

    public class requestBaseDTO
    {
        public string jsonrpc { get; set; }
        public string method { get; set; }

        [JsonPropertyName("params")]
        public object[] _params { get; set; }
        public int id { get; set; }
    }

}
