
using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using eth_shared.Processors;

using Microsoft.EntityFrameworkCore;

using nethereum;

using System.Collections.Concurrent;

namespace eth_shared
{
    public class Step1
    {
        private ConcurrentBag<getTokenMetadataDTO> tokenMetadataUnfiltered = new();
        private List<getTransactionReceiptDTO> txnReceiptsUnfiltered = new();
        private List<getTotalSupplyDTO> totalSupplyDTOFiltered = new();

        private List<Transaction> tokens = new();
        private List<getBlockByNumberDTO> blocksFiltered = new();
        private List<getTokenMetadataDTO> tokenMetadataFiltered = new();
        private List<getTransactionReceiptDTO.Result> txnReceiptsFiltered = new();

        private List<EthBlocks> ethBlocks = new();
        private List<EthTrainData> ethTrainDatas = new();

        private readonly GetTokenMetadata getTokenMetadata;
        private readonly GetTransactionReceipt getTransactionReceipt;
        private readonly GetBlocks getBlocks;
        private readonly GetTransactions getTransactions;
        private readonly ApiWeb3 apiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly GetTotalSupply getTotalSupply;
        private readonly dbContext dbContext;
        private readonly ProcessorGeneral processorGeneral;

        private readonly int batchSize = 50;
        private readonly int maxDiffToProcess = 250;

        public Step1(
            ApiWeb3 apiWeb3,
            EthApi apiAlchemy,
            dbContext dbContext,
            GetBlocks getBlocks,
            GetTotalSupply getTotalSupply,
            GetTransactions getTransactions,
            GetTokenMetadata getTokenMetadata,
            GetTransactionReceipt getTransactionReceipt,
            ProcessorGeneral processorGeneral
            )
        {
            this.getBlocks = getBlocks;
            this.apiWeb3 = apiWeb3;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
            this.getTotalSupply = getTotalSupply;
            this.getTransactions = getTransactions;
            this.getTokenMetadata = getTokenMetadata;
            this.getTransactionReceipt = getTransactionReceipt;
            this.processorGeneral = processorGeneral;
        }
        public async Task Start()
        {
            var lastBlockNumber = await GetLastEthBlockNumber();
            var lastProccessedBlock = await GetLastProccessedBlockNumber();

            //var test = await GetTransactionsFromBlockByNumber(20350220);
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

        private async Task<int> GetLastEthBlockNumber()
        {
            //var res = await apiAlchemy.lastBlockNumber();
            var res = 20384889;

            return res;
        }


        private async Task<getBlockByNumberDTO> GetTransactionsFromBlockByNumber(int block)
        {
            var res = await apiAlchemy.getBlockByNumber(block);

            return res;
        }

        private async Task<List<getBlockByNumberDTO>> GetTransactionsFromBlockByNumberBatch(List<int> blocks)
        {
            var res = await apiAlchemy.getBlockByNumberBatch(blocks, 0);

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
