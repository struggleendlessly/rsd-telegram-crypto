using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using eth_shared.Map;

using Microsoft.EntityFrameworkCore;

namespace eth_shared
{
    public class GetBlocks
    {
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;

        private List<getBlockByNumberDTO> validated = new();
        public GetBlocks(
            EthApi apiAlchemy,
            dbContext dbContext
            )
        {
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
        }

        public async Task<List<Transaction>> Start(
            int lastBlockNumber,
            int lastProcessedBlock)
        {
            var unfiltered = await Get(lastBlockNumber, lastProcessedBlock);
            validated = Validate(unfiltered);
            var res = Filter(validated);

            return res;
        }

        public async Task<int> End()
        {
            var distinct = await DictinctBlocksInDB();
            var res = await SaveToDB(distinct);

            return res;
        }

        private async Task<List<getBlockByNumberDTO>> Get(
            int lastBlockNumber,
            int lastProcessedBlock)
        {
            List<getBlockByNumberDTO> res = new();

            var diff = lastBlockNumber - lastProcessedBlock;

            var items = Enumerable.Range(lastProcessedBlock, diff).ToList();

            Func<List<int>, int, Task<List<getBlockByNumberDTO>>> apiMethod = apiAlchemy.getBlockByNumberBatch;

            res = await apiAlchemy.executeBatchCall(items, apiMethod, diff);

            return res;
        }

        public List<getBlockByNumberDTO> Validate(List<getBlockByNumberDTO> collection)
        {
            List<getBlockByNumberDTO> res = new();

            foreach (var item in collection)
            {
                if (item.result is not null &&
                    item.result.transactions is not null)
                {
                    res.Add(item);
                }
            }

            res = res.DistinctBy(x => x.result.number).ToList();

            return res;
        }

        // 18160ddd - totalSupply()
        public List<Transaction> Filter(List<getBlockByNumberDTO> collection)
        {
            List<Transaction> res = new();
            List<Transaction> transactions = new();

            foreach (var item in collection)
            {
                transactions.AddRange(item.result.transactions);
            }

            foreach (var item in transactions)
            {
                if (string.IsNullOrEmpty(item.to) &&
                    item.input.Contains("18160ddd"))
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public async Task<List<EthBlocks>> DictinctBlocksInDB()
        {
            List<EthBlocks> res = new();

            foreach (var item in validated)
            {
                var t = item.Map();

                if (string.IsNullOrEmpty(t.number))
                {
                    continue;
                }

                t.numberInt = Convert.ToInt32(t.number, 16);

                var isExist = await dbContext.EthBlock.AnyAsync(x => x.numberInt == t.numberInt);

                if (!isExist)
                {
                    res.Add(t);
                }
            }

            return res;
        }

        private async Task<int> SaveToDB(List<EthBlocks> ethBlocks)
        {
            var res = 0;

            dbContext.EthBlock.AddRange(ethBlocks);
            res = await dbContext.SaveChangesAsync();

            return res;
        }
    }
}
