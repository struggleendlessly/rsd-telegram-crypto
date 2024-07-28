
using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using eth_shared.Filters;
using eth_shared.Map;
using eth_shared.Processors;

using Microsoft.EntityFrameworkCore;

using nethereum;

using System.Collections.Concurrent;

namespace eth_shared
{
    public class FindTransactionService
    {
        private ConcurrentBag<getBlockByNumberDTO> blocksUnfiltered = new();
        private ConcurrentBag<getTokenMetadataDTO> tokenMetadataUnfiltered = new();
        private ConcurrentBag<getTransactionReceiptDTO.Result> txnReceiptsUnfiltered = new();

        private List<Transaction> tokens = new();
        private List<Transaction> others = new();
        private List<getBlockByNumberDTO> blocksFiltered = new();
        private List<getTokenMetadataDTO> tokenMetadataFiltered = new();
        private List<getTransactionReceiptDTO.Result> txnReceiptsFiltered = new();

        private List<EthBlocks> ethBlocks = new();
        private List<EthTrainData> ethTrainDatas = new();

        private readonly ApiWeb3 apiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        private readonly ProcessorGeneral processorGeneral;

        private readonly int batchSize = 50;
        private readonly int maxDiffToProcess = 250;

        public FindTransactionService(
            ApiWeb3 apiWeb3,
            EthApi apiAlchemy,
            dbContext dbContext,
            ProcessorGeneral processorGeneral
            )
        {
            this.apiWeb3 = apiWeb3;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
            this.processorGeneral = processorGeneral;
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

        // Order is important !!!
        public async Task Middle()
        {
            blocksFiltered = FilterBlock.Filter_EmptyBlocks_Distinct(blocksUnfiltered);
            tokens = FilterTx.FilterTokens_ToIsNull_TotalSupply(blocksFiltered);

            await GetTransactionReceipt();
            txnReceiptsFiltered = FilterTx.FilterTxnReceipts_LogsCount(txnReceiptsUnfiltered);

            await GetTokenMetadata();
            tokenMetadataFiltered = FilterTx.FilterMetadata_Names(tokenMetadataUnfiltered);

            ethTrainDatas = await processorGeneral.ProcessTokens(tokens, txnReceiptsFiltered, tokenMetadataFiltered);
            ethBlocks = await processorGeneral.ProcessBlocks(blocksFiltered);

            await GetTotalSupply();
            ethTrainDatas = FilterTx.FilterTotalSupply(ethTrainDatas);
        }

        public async Task End()
        {
            await SaveToDB_Txc();
            //await SaveToDB_Others();
            await SaveToDB_Blocks();
        }

        private async Task<int> GetLastEthBlockNumber()
        {
            //var res = await apiAlchemy.lastBlockNumber();
            var res = 20384889;

            return res;
        }

        private async Task GetTotalSupply()
        {
            foreach (var item in ethTrainDatas)
            {
                var t = await apiWeb3.GetTotalSupply(item.contractAddress);

                for (int i = 0; i < item.decimals; i++)
                {
                    t = t / 10;
                }

                item.totalSupply = t.ToString();

                Thread.Sleep(300);
            }
        }

        private async Task GetBlocks(
            int lastBlockNumber,
            int lastProccessedBlock)
        {
            var rangeOfBatches = 1;
            var diff = lastBlockNumber - lastProccessedBlock;

            if (diff > 0)
            {
                var batchSizeLocal = batchSize;

                if (diff > maxDiffToProcess)
                {
                    diff = maxDiffToProcess;
                }

                rangeOfBatches = (int)Math.Floor(diff / (double)batchSize);

                if (rangeOfBatches == 0)
                {
                    rangeOfBatches = 1;
                    batchSizeLocal = diff;
                }

                List<int> rangeForBatches = Enumerable.Range(0, rangeOfBatches).ToList();
                List<int> rangeForBlocksFull = Enumerable.Range(lastProccessedBlock, diff).ToList();
                var rangeForBlocksChunks = rangeForBlocksFull.Chunk(batchSizeLocal).ToList();

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

        public async Task<int> GetLastProccessedBlockNumber()
        {
            var res = 18910000;

            if ((await dbContext.EthBlock.CountAsync()) > 0)
            {
                res = await dbContext.EthBlock.MaxAsync(x => x.numberInt);
            }

            return res;
        }

        private async Task GetTransactionReceipt()
        {
            List<getTransactionReceiptDTO.Result> transactions = new();

            var diff = tokens.Count();

            if (diff > 0)
            {
                var batchSizeLocal = batchSize;

                if (diff > maxDiffToProcess)
                {
                    diff = maxDiffToProcess;
                }

                var rangeOfBatches = (int)Math.Floor(diff / (double)batchSize);

                if (rangeOfBatches == 0)
                {
                    rangeOfBatches = 1;
                    batchSizeLocal = diff;
                }

                List<int> rangeForBatches = Enumerable.Range(0, rangeOfBatches).ToList();
                var rangeChunks = tokens.Chunk(batchSizeLocal).ToList();

                await Parallel.ForEachAsync(
                    rangeForBatches,
                    new ParallelOptions { MaxDegreeOfParallelism = 4, },
                    async (iterator, ct) =>
                    {
                        var chunk = rangeChunks[iterator].Select(x => x.hash).ToList();

                        var t = await apiAlchemy.getTransactionReceiptBatch(chunk);

                        foreach (var item in t)
                        {
                            txnReceiptsUnfiltered.Add(item.result);
                        }

                        Thread.Sleep(50);
                    });
            }
        }

        private async Task GetTokenMetadata()
        {
            List<getTokenMetadataDTO.Result> transactions = new();

            var diff = tokens.Count();

            if (diff > 0)
            {
                var batchSizeLocal = batchSize;

                if (diff > maxDiffToProcess)
                {
                    diff = maxDiffToProcess;
                }

                var rangeOfBatches = (int)Math.Floor(diff / (double)batchSize);

                if (rangeOfBatches == 0)
                {
                    rangeOfBatches = 1;
                    batchSizeLocal = diff;
                }

                for (int i = 0; i < txnReceiptsFiltered.Count; i++)
                {
                    var item = txnReceiptsFiltered[i];
                    item.txnNumberForMetadata = i;
                }

                List<int> rangeForBatches = Enumerable.Range(0, rangeOfBatches).ToList();
                var rangeChunks = txnReceiptsFiltered.Chunk(batchSizeLocal).ToList();

                await Parallel.ForEachAsync(
                    rangeForBatches,
                    new ParallelOptions { MaxDegreeOfParallelism = 4, },
                    async (iterator, ct) =>
                    {
                        var chunk = rangeChunks[iterator].ToList();

                        var t = await apiAlchemy.getTokenMetadataBatch(chunk);

                        foreach (var item in t)
                        {
                            tokenMetadataUnfiltered.Add(item);
                        }

                        Thread.Sleep(50);
                    });
            }
        }

        private async Task<int> SaveToDB_Txc()
        {
            var res = 0;

            dbContext.EthTrainData.AddRange(ethTrainDatas);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        //private async Task<int> SaveToDB_Others()
        //{
        //    var res = 0;

        //    var t = ProcessOthers();

        //    dbContext.EthTrxOther.AddRange(t);
        //    res = await dbContext.SaveChangesAsync();

        //    return res;
        //}

        private async Task<int> SaveToDB_Blocks()
        {
            var res = 0;

            dbContext.EthBlock.AddRange(ethBlocks);
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

            var rangeOfBatches = 1;
            var diff = blockDiff.Count();

            if (diff > 0)
            {
                var batchSizeLocal = batchSize;

                if (diff > maxDiffToProcess)
                {
                    diff = maxDiffToProcess;
                }

                rangeOfBatches = (int)Math.Floor(diff / (double)batchSize);

                if (rangeOfBatches == 0)
                {
                    rangeOfBatches = 1;
                    batchSizeLocal = diff;
                }

                List<int> rangeForBatches = Enumerable.Range(0, rangeOfBatches).ToList();
                var blockDiffChunks = blockDiff.Chunk(batchSizeLocal).ToList();

                await GetBlocksInParalel(rangeForBatches, blockDiffChunks);

                await Middle();
                await End();
            }
        }
    }
}
