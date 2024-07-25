
using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using eth_shared.Map;

using Microsoft.EntityFrameworkCore;

using System.Collections.Concurrent;

namespace eth_shared
{
    public class FindTransactionService
    {
        private ConcurrentBag<getBlockByNumberDTO> blocksUnfiltered = new();
        private List<getBlockByNumberDTO> blocksFiltered = new();
        private List<Transaction> tokens = new();
        private List<Transaction> others = new();

        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;

        private readonly int maxDiffToProcess = 250;
        private int batchSize = 50;

        public FindTransactionService(
            EthApi apiAlchemy,
            dbContext dbContext
            )
        {
            this.apiAlchemy = apiAlchemy;
            this.dbContext = dbContext;
        }
        public async Task Start()
        {
            //await CheckSkippedBlocks();
            var lastBlockNumber = await GetLastEthBlockNumber();
            var lastProccessedBlock = await GetLastProccessedBlockNumber();

            //var test = await GetTransactionsFromBlockByNumber(20350220);
            await GetBlocks(lastBlockNumber, lastProccessedBlock);
            await Middle();
            await End();

            await CheckSkippedBlocks();
        }

        public async Task Middle()
        {
            await FilterReceivedBlocks();
            await ApplyFiltersToTransactions();
        }

        public async Task End()
        {
            await SaveToDB_Txc();
            //await SaveToDB_Others();
            await SaveToDB_Blocks();
        }


        private async Task FilterReceivedBlocks()
        {
            foreach (var item in blocksUnfiltered)
            {
                if (item.result is not null && item.result.transactions is not null)
                {
                    blocksFiltered.Add(item);
                }
            }

            blocksFiltered = blocksFiltered.DistinctBy(x => x.result.number).ToList();
        }

        private async Task<int> GetLastEthBlockNumber()
        {
            var res = await apiAlchemy.lastBlockNumber();

            return res;
        }

        private async Task GetBlocks(
            int lastBlockNumber,
            int lastProccessedBlock)
        {
            var rangeOfBatches = 1;
            var diff = lastBlockNumber - lastProccessedBlock;

            if (diff > 0)
            {
                if (diff > maxDiffToProcess)
                {
                    diff = maxDiffToProcess;
                }

                rangeOfBatches = (int)Math.Floor(diff / (double)batchSize);

                if (rangeOfBatches == 0)
                {
                    rangeOfBatches = 1;
                    batchSize = diff;
                }

                List<int> rangeForBatches = Enumerable.Range(0, rangeOfBatches).ToList();
                List<int> rangeForBlocksFull = Enumerable.Range(lastProccessedBlock, diff).ToList();
                var rangeForBlocksChunks = rangeForBlocksFull.Chunk(batchSize).ToList();

                await GetBlocksInParalel(rangeForBatches, rangeForBlocksChunks);
            }
        }

        private async Task GetBlocksInParalel(
            List<int> rangeForBatches,
            List<int[]> rangeForBlocksChunks)
        {

            await Parallel.ForEachAsync(
                rangeForBatches,
                new ParallelOptions { MaxDegreeOfParallelism = 4, },
                async (iterator, ct) =>
                {
                    List<int> rangeForBlocks = rangeForBlocksChunks[iterator].ToList();

                    var t = await GetTransactionsFromBlockByNumberBatch(rangeForBlocks);

                    foreach (var item in t)
                    {
                        blocksUnfiltered.Add(item);
                    }

                    Thread.Sleep(50);
                });

        }

        private async Task<getBlockByNumberDTO> GetTransactionsFromBlockByNumber(int block)
        {
            var res = await apiAlchemy.getBlockByNumber(block);

            return res;
        }

        private async Task<List<getBlockByNumberDTO>> GetTransactionsFromBlockByNumberBatch(List<int> blocks)
        {
            var res = await apiAlchemy.getBlockByNumberBatch(blocks);

            return res;
        }


        private async Task<int> GetLastProccessedBlockNumber()
        {
            var res = 18910000;

            if ((await dbContext.EthBlock.CountAsync()) > 0)
            {
                res = await dbContext.EthBlock.MaxAsync(x => x.numberInt);
            }

            return res;
        }

        // 18160ddd - totalSupply()
        private async Task ApplyFiltersToTransactions()
        {
            List<Transaction> transactions = new();

            foreach (var item in blocksFiltered)
            {
                transactions.AddRange(item.result.transactions);
            }

            foreach (var item in transactions)
            {
                if (string.IsNullOrEmpty(item.to) && item.input.Contains("18160ddd"))
                {
                    tokens.Add(item);
                }
                else
                {
                    others.Add(item);
                }
            }
        }

        private List<EthTrainData> ProcessTokens()
        {
            List<EthTrainData> res = new();

            foreach (var item in tokens)
            {
                var t = item.Map();

                if (t.input.StartsWith("0x6080") ||
                    t.input.StartsWith("0x6040")
                    )
                {
                    t.isCustomInputStart = false;
                }
                else
                {
                    t.isCustomInputStart = true;
                }

                t.blockNumberInt = Convert.ToInt32(t.blockNumber, 16);

                var isExist = dbContext.EthTrainData.Any(x => x.hash == t.hash);

                if (!isExist)
                {
                    res.Add(t);
                }
            }

            return res;
        }

        private List<EthTrxOthers> ProcessOthers()
        {
            List<EthTrxOthers> res = new();

            foreach (var item in others)
            {
                var t = item.MapOthers();

                t.blockNumberInt = Convert.ToInt32(t.blockNumber, 16);

                var isExist = dbContext.EthTrxOther.Any(x => x.hash == t.hash);

                if (!isExist)
                {
                    res.Add(t);
                }
            }

            return res;
        }

        private List<EthBlocks> ProcessBlocks()
        {
            List<EthBlocks> res = new();

            foreach (var item in blocksFiltered)
            {
                var t = item.Map();

                if (string.IsNullOrEmpty(t.number))
                {
                    continue;
                }

                t.numberInt = Convert.ToInt32(t.number, 16);

                var isExist = dbContext.EthBlock.Any(x => x.number == t.number);

                if (!isExist)
                {
                    res.Add(t);
                }
            }

            return res;
        }

        private async Task<int> SaveToDB_Txc()
        {
            var res = 0;

            var t = ProcessTokens();

            dbContext.EthTrainData.AddRange(t);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        private async Task<int> SaveToDB_Others()
        {
            var res = 0;

            var t = ProcessOthers();

            dbContext.EthTrxOther.AddRange(t);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        private async Task<int> SaveToDB_Blocks()
        {
            var res = 0;

            var t = ProcessBlocks();

            dbContext.EthBlock.AddRange(t);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        private async Task CheckSkippedBlocks()
        {
            blocksUnfiltered = new();
            blocksFiltered = new();
            tokens = new();
            others = new();

            var blocks = dbContext.EthBlock.Select(x => x.numberInt).ToList().Order().ToList();
            var minBlockNumber = blocks.First();
            var lastProccessedBlock = await GetLastProccessedBlockNumber();
            var endOfEtalonRange = lastProccessedBlock - minBlockNumber;
            var etalonBlockNumbers = Enumerable.Range(minBlockNumber, endOfEtalonRange);
            bool isInSequence = blocks.SequenceEqual(etalonBlockNumbers);
            var blockDiff = etalonBlockNumbers.Except(blocks).ToList();
            var blockDiffChunks = blockDiff.Chunk(batchSize).ToList();

            var rangeOfBatches = 1;
            var diff = blockDiff.Count();

            if (diff > 0)
            {
                if (diff > maxDiffToProcess)
                {
                    diff = maxDiffToProcess;
                }

                rangeOfBatches = (int)Math.Floor(diff / (double)batchSize);

                if (rangeOfBatches == 0)
                {
                    rangeOfBatches = 1;
                    batchSize = diff;
                }

                List<int> rangeForBatches = Enumerable.Range(0, rangeOfBatches).ToList();

                await GetBlocksInParalel(rangeForBatches, blockDiffChunks);
            }

            await Middle();
            await End();
        }
    }
}
