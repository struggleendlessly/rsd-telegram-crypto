

using api_alchemy.Eth.RequestDTO;

using System.Text.Json;

namespace api_alchemy.Eth
{
    public static class EthUrlBuilder
    {

        public static string getBlockByNumber(int block)
        {
            var hexBlock = "0x" + block.ToString("X");

            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "eth_getBlockByNumber",
                _params = [$"{hexBlock}", true],
                id = 0
            };

            //var res = $$"""
            //        {
            //            "jsonrpc":"2.0",
            //            "method":"eth_getBlockByNumber",
            //            "params":["{{hexBlock}}", true],
            //            "id":0
            //        }
            //    """;

            var res = JsonSerializer.Serialize(request);

            return res;
        }
    }
}
