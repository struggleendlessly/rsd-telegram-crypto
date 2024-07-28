using api_alchemy.Eth.RequestDTO;
using api_alchemy.Eth.RequestDTO.Params;

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

        public static string getTokenMetadata(
            string contractAddress,
            int id)
        {
            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "alchemy_getTokenMetadata",
                _params = [$"{contractAddress}"],
                id = id
            };

            var res = JsonSerializer.Serialize(request);

            return res;
        }    
        
        public static string eth_call(
            string contractAddress,
            int id,
            string methodCode)
        {
            var paramDto = new totalSupplyDTO()
            {
                to = contractAddress,
                data = methodCode
            };

            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "eth_call",
                _params = [paramDto],
                id = id
            };

            var res = JsonSerializer.Serialize(request);

            return res;
        }
    }
}
