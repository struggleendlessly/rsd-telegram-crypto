using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public sealed class WorkerScoped : IScopedProcessingService
    {
        private readonly ILogger _logger;

        private List<Transaction> tokens = new();
        private List<getTokenMetadataDTO> tokenMetadataFiltered = new();
        private List<getTotalSupplyDTO> totalSupplyDTOFiltered = new();
        private List<getTransactionReceiptDTO.Result> txnReceiptsFiltered = new();

        private List<EthTrainData> ethTrainDatas = new();

        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        private readonly GetBlocks getBlocks;
        private readonly GetTotalSupply getTotalSupply;
        private readonly GetTransactions getTransactions;
        private readonly GetTokenMetadata getTokenMetadata;
        private readonly GetTransactionReceipt getTransactionReceipt;

        private int _executionCount;

        public WorkerScoped(
            ILogger<WorkerScoped> logger,
            EthApi apiAlchemy,
            dbContext dbContext,
            GetBlocks getBlocks,
            GetTotalSupply getTotalSupply,
            GetTransactions getTransactions,
            GetTokenMetadata getTokenMetadata,
            GetTransactionReceipt getTransactionReceipt
            )
        {
            this._logger = logger;
            this.getBlocks = getBlocks;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
            this.getTotalSupply = getTotalSupply;
            this.getTransactions = getTransactions;
            this.getTokenMetadata = getTokenMetadata;
            this.getTransactionReceipt = getTransactionReceipt;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation("Worker WorkerScoped running at: {time}", DateTimeOffset.Now);
                var lastBlock = await dbContext.EthTrainData.OrderByDescending(x => x.blockNumberInt).FirstAsync();
                _logger.LogInformation("Worker WorkerScoped lastBlock proccessed before: {number}", lastBlock.blockNumberInt);
                var timeStart = DateTimeOffset.Now;

                try
                {
                    await Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker WorkerScoped Exception: {message}", ex.Message);
                    _logger.LogError("Worker WorkerScoped Exception: {stack}", ex.StackTrace);
                }

                var timeEnd = DateTimeOffset.Now;

                lastBlock = await dbContext.EthTrainData.OrderByDescending(x => x.blockNumberInt).FirstAsync();
                _logger.LogInformation("Worker WorkerScoped lastBlock proccessed after: {number}", lastBlock.blockNumberInt);
                _logger.LogInformation("Worker WorkerScoped running time: {time}", (timeEnd - timeStart).TotalSeconds);

                await Task.Delay(60000, stoppingToken);
            }
        }

        async Task Start()
        {
            var lastBlockNumber = await apiAlchemy.lastBlockNumber();
            var lastProccessedBlock = await GetLastProccessedBlockNumber();

            _logger.LogInformation("Worker WorkerScoped lastBlockNumber: {number}", lastBlockNumber);
            _logger.LogInformation("Worker WorkerScoped lastProccessedBlock: {number}", lastProccessedBlock);

            tokens = await getBlocks.Start(lastBlockNumber, lastProccessedBlock);

            await Middle();
            await End();

            //await CheckSkippedBlocks();
        }

        // Order is important !!!
        async Task Middle()
        {
            txnReceiptsFiltered = await getTransactionReceipt.Start(tokens);
            tokenMetadataFiltered = await getTokenMetadata.Start(txnReceiptsFiltered);
            totalSupplyDTOFiltered = await getTotalSupply.Start(txnReceiptsFiltered, tokenMetadataFiltered);

            ethTrainDatas = await getTransactions.Start(
                tokens,
                txnReceiptsFiltered,
                tokenMetadataFiltered,
                totalSupplyDTOFiltered);

        }

        async Task End()
        {
            await getTransactions.End(ethTrainDatas);
            await getBlocks.End();
        }
        async Task<int> GetLastProccessedBlockNumber()
        {
            var res = 18910000;

            if ((await dbContext.EthBlock.CountAsync()) > 0)
            {
                res = await dbContext.EthBlock.MaxAsync(x => x.numberInt);
            }

            return res;
        }
        //private async Task CheckSkippedBlocks()
        //{
        //    var blocks = dbContext.EthBlock.Select(x => x.numberInt).ToList().Order().ToList();
        //    var minBlockNumber = blocks.First();
        //    var lastProccessedBlock = await GetLastProccessedBlockNumber();
        //    var endOfEtalonRange = lastProccessedBlock - minBlockNumber;
        //    var etalonBlockNumbers = Enumerable.Range(minBlockNumber, endOfEtalonRange);
        //    bool isInSequence = blocks.SequenceEqual(etalonBlockNumbers);
        //    var blockDiff = etalonBlockNumbers.Except(blocks).ToList();

        //    var rangeOfBatches = 1;
        //    //var diff = blockDiff.Count();

        //    //if (diff > 0)
        //    //{
        //    //    var batchSizeLocal = batchSize;

        //    //    if (diff > maxDiffToProcess)
        //    //    {
        //    //        diff = maxDiffToProcess;
        //    //    }

        //    //    rangeOfBatches = (int)Math.Floor(diff / (double)batchSize);

        //    //    if (rangeOfBatches == 0)
        //    //    {
        //    //        rangeOfBatches = 1;
        //    //        batchSizeLocal = diff;
        //    //    }

        //    //    List<int> rangeForBatches = Enumerable.Range(0, rangeOfBatches).ToList();
        //    //    var blockDiffChunks = blockDiff.Chunk(batchSizeLocal).ToList();

        //    //    await GetBlocksInParalel(rangeForBatches, blockDiffChunks);

        //    //    await Middle();
        //    //    await End();
        //    //}
        //}
    }
}
