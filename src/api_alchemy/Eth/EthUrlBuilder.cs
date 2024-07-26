using api_alchemy.Eth.RequestDTO;

using System.Text.Json;

namespace api_alchemy.Eth
{
    public static class EthUrlBuilder
    {
        public static string lastBlockNumber()
        {
            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "eth_blockNumber",
                id = 0
            };

            var res = JsonSerializer.Serialize(request);

            return res;
        }

        public static string getBlockByNumber(int block)
        {
            var hexBlock = "0x" + block.ToString("X");

            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "eth_getBlockByNumber",
                _params = [$"{hexBlock}", true],
                id = block
            };

            var res = JsonSerializer.Serialize(request);

            return res;
        }

        public static string getTransactionReceipt(string transactionHash)
        {
            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "eth_getTransactionReceipt",
                _params = [$"{transactionHash}"],
                id = 0
            };

            var res = JsonSerializer.Serialize(request);

            return res;
        }

        public static string getTokenMetadata(string contractAddress)
        {
            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "alchemy_getTokenMetadata",
                _params = [$"{contractAddress}"],
                id = 0
            };

            var res = JsonSerializer.Serialize(request);

            return res;
        }
    }
}
