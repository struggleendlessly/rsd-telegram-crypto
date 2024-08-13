using api_alchemy.Eth.RequestDTO;
using api_alchemy.Eth.RequestDTO.Params;

using System.Text.Json;

using TL.Methods;

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

        public static string alchemy_getAssetTransfers(
            string fromAddress,
            string blockNumber,
            string category = "external")
        {
            var paramDto = new getAssetTransfersDTO()
            {
                category = [category],
                excludeZeroValue = true,
                toBlock = blockNumber,
                maxCount = "0x1",
                toAddress = fromAddress,
                withMetadata = true,
                fromBlock = "0x0"
            };

            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "alchemy_getAssetTransfers",
                _params = [paramDto],
                id = 1
            };

            var res = JsonSerializer.Serialize(request);

            //Thread.Sleep(500);

            return res;
        }

        public static string getBalance(
            string fromAddress,
            int id,
            string blockNumber)
        {
            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "eth_getBalance",
                _params = [$"{fromAddress}", $"{blockNumber}"],
                id = id
            };

            var res = JsonSerializer.Serialize(request);

            return res;
        }
        public static string getSwapLogs(
            string pairAddress,
            string topic,
            string blockNumberStart,
            string blockNumberEnd)
        {
            var paramDto = new getSwapDTO()
            {
                topics = [topic],
                address = [pairAddress],
                fromBlock = blockNumberStart,
                toBlock = blockNumberEnd
            };


            var request = new requestBaseDTO()
            {
                jsonrpc = "2.0",
                method = "eth_getLogs",
                _params = [paramDto],
                id = 1
            };

            var res = JsonSerializer.Serialize(request);

            return res;
        }
    }
}
