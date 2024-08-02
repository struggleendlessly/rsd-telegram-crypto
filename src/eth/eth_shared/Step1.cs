
using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using nethereum;

namespace eth_shared
{
    public class Step1
    {
        private List<Transaction> tokens = new();
        private List<getTokenMetadataDTO> tokenMetadataFiltered = new();
        private List<getTotalSupplyDTO> totalSupplyDTOFiltered = new();
        private List<getTransactionReceiptDTO.Result> txnReceiptsFiltered = new();

        private List<EthTrainData> ethTrainDatas = new();
        
        private readonly ILogger logger;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        private readonly GetBlocks getBlocks;
        private readonly GetTotalSupply getTotalSupply;
        private readonly GetTransactions getTransactions;
        private readonly GetTokenMetadata getTokenMetadata;
        private readonly GetTransactionReceipt getTransactionReceipt;

        public Step1(
            ILogger<Step1> logger,
            EthApi apiAlchemy,
            dbContext dbContext,
            GetBlocks getBlocks,
            GetTotalSupply getTotalSupply,
            GetTransactions getTransactions,
            GetTokenMetadata getTokenMetadata,
            GetTransactionReceipt getTransactionReceipt
            )
        {
            this.logger = logger;
            this.getBlocks = getBlocks;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
            this.getTotalSupply = getTotalSupply;
            this.getTransactions = getTransactions;
            this.getTokenMetadata = getTokenMetadata;
            this.getTransactionReceipt = getTransactionReceipt;
        }
        public async Task Start()
        {
            var lastBlockNumber = await apiAlchemy.lastBlockNumber();
            var lastProccessedBlock = await GetLastProccessedBlockNumber();

            tokens = await getBlocks.Start(lastBlockNumber, lastProccessedBlock);

            await Middle();
            await End();

            //await CheckSkippedBlocks();
        }

        // Order is important !!!
        public async Task Middle()
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

        public async Task End()
        {
            await getTransactions.End(ethTrainDatas);
            await getBlocks.End();
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


        private async Task CheckSkippedBlocks()
        {
            var blocks = dbContext.EthBlock.Select(x => x.numberInt).ToList().Order().ToList();
            var minBlockNumber = blocks.First();
            var lastProccessedBlock = await GetLastProccessedBlockNumber();
            var endOfEtalonRange = lastProccessedBlock - minBlockNumber;
            var etalonBlockNumbers = Enumerable.Range(minBlockNumber, endOfEtalonRange);
            bool isInSequence = blocks.SequenceEqual(etalonBlockNumbers);
            var blockDiff = etalonBlockNumbers.Except(blocks).ToList();

            var rangeOfBatches = 1;
            //var diff = blockDiff.Count();

            //if (diff > 0)
            //{
            //    var batchSizeLocal = batchSize;

            //    if (diff > maxDiffToProcess)
            //    {
            //        diff = maxDiffToProcess;
            //    }

            //    rangeOfBatches = (int)Math.Floor(diff / (double)batchSize);

            //    if (rangeOfBatches == 0)
            //    {
            //        rangeOfBatches = 1;
            //        batchSizeLocal = diff;
            //    }

            //    List<int> rangeForBatches = Enumerable.Range(0, rangeOfBatches).ToList();
            //    var blockDiffChunks = blockDiff.Chunk(batchSizeLocal).ToList();

            //    await GetBlocksInParalel(rangeForBatches, blockDiffChunks);

            //    await Middle();
            //    await End();
            //}
        }
    }
}
