using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Microsoft.Extensions.Logging;
using nethereum;

namespace eth_shared
{
    public class GetTransactionReceipt
    {
        private readonly ILogger logger;
        private readonly EthApi apiAlchemy;
        public GetTransactionReceipt(
            ILogger<GetTransactionReceipt> logger,
            EthApi apiAlchemy)
        {
            this.logger = logger;
            this.apiAlchemy = apiAlchemy;
        }

        public async Task<List<getTransactionReceiptDTO.Result>> Start(
            List<Transaction> tokens)
        {
            Thread.Sleep(1000);

            var unfiltered = await Get(tokens);
            var validated = Validate(unfiltered);
            var res = Filter(validated);

            return res;
        }

        private async Task<List<getTransactionReceiptDTO>> Get(List<Transaction> tokens)
        {
            List<getTransactionReceiptDTO> res = new();

            var diff = tokens.Count();
            var items = tokens.Select(x => x.hash).ToList();

            Func<List<string>, int, Task<List<getTransactionReceiptDTO>>> apiMethod = apiAlchemy.getTransactionReceiptBatch;

            res = await apiAlchemy.executeBatchCall(items, apiMethod, diff);

            return res;
        }

        public List<getTransactionReceiptDTO> Validate(List<getTransactionReceiptDTO> collection)
        {
            List<getTransactionReceiptDTO> res = new();

            foreach (var item in collection)
            {
                if (item.result is not null)
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public List<getTransactionReceiptDTO.Result> Filter(List<getTransactionReceiptDTO> collection)
        {
            List<getTransactionReceiptDTO.Result> res = new();

            foreach (var item in collection)
            {
                if (item.result.logs.Count() > 1)
                {
                    res.Add(item.result);
                }
            }

            return res;
        }
    }
}
