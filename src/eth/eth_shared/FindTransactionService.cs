
using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using System.Collections.Concurrent;

namespace eth_shared
{
    public class FindTransactionService
    {

        private ConcurrentBag<Transaction> tokens = new ConcurrentBag<Transaction>();
        private ConcurrentBag<Transaction> others = new ConcurrentBag<Transaction>();
        private readonly EthApi apiAlchemy;
        public FindTransactionService(
            EthApi apiAlchemy
            )
        {
            this.apiAlchemy = apiAlchemy;
        }
        public async Task Start()
        {
            var lastBlockNumber = await GetLastEthBlockNumber();
            var lastProccessedBlock = await GetLastProccessedBlockNumber();
            var block = await GetTransactionsFromBlocks(lastBlockNumber, lastProccessedBlock);
            //await ApplyFiltersToTransactions(block.result.transactions);
            //SaveToDB();
        }

        private async Task<int> GetLastEthBlockNumber()
        {
            var res = await apiAlchemy.lastBlockNumber();

            return res;
        }

        private async Task<List<Transaction>> GetTransactionsFromBlocks(
            int lastBlockNumber,
            int lastProccessedBlock)
        {
            ConcurrentBag<Transaction> res = new ConcurrentBag<Transaction>();

            var diff = lastBlockNumber - lastProccessedBlock;
            var endBlock = lastBlockNumber;

            if (diff > 0)
            {
                if (diff > 100)
                {
                    endBlock = lastProccessedBlock + 100;
                }
                else
                {
                    endBlock = lastProccessedBlock + diff;
                }
                List<int> range = Enumerable.Range(lastProccessedBlock, endBlock).ToList();
                Parallel.ForEach(range, new ParallelOptions { MaxDegreeOfParallelism = 4 }, async (block) =>
                {
                    var t = await GetTransactionsFromBlockByNumber(block);

                    foreach (var item in t.result.transactions)
                    {
                        res.Add(item);
                    }
                });

            }

            return res.ToList();
        }
        private async Task<getBlockByNumberDTO> GetTransactionsFromBlockByNumber(int block)
        {
            var res = await apiAlchemy.getBlockByNumber(block);

            return res;
        }

        private async Task<int> GetLastProccessedBlockNumber()
        {
            var res = 20368085;

            return res;
        }

        // 18160ddd - totalSupply()
        private async Task ApplyFiltersToTransactions(Transaction[] transactions)
        {
            foreach (var item in transactions)
            {
                if (string.IsNullOrEmpty(item.to)/* && item.input.Contains("18160ddd")*/)
                {
                    tokens.Add(item);
                }
                else
                {
                    others.Add(item);
                }
            }
        }
    }
}
