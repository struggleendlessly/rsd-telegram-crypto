using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using etherscan;
using etherscan.ResponseDTO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using nethereum;

namespace eth_shared
{
    public class GetPair
    {

        private readonly ILogger logger;
        private readonly ApiWeb3 ApiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        private readonly EtherscanApi etherscanApi;

        int lastEthBlockNumber = 0;
        public GetPair(
            ILogger<GetPair> logger,
            ApiWeb3 ApiWeb3,
            EthApi apiAlchemy,
            dbContext dbContext,
            EtherscanApi etherscanApi
            )
        {
            this.logger = logger;
            this.ApiWeb3 = ApiWeb3;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
            this.etherscanApi = etherscanApi;
        }

        public async Task Start()
        {
            lastEthBlockNumber = (await dbContext.EthBlock.OrderByDescending(x => x.numberInt).Take(1).SingleAsync()).numberInt;

            var openTradingStr = "openTrading";
            var addLiquidityStr = "addLiquidity";

            var tokensToProcess = await GetTokensToProcess();
            var unverified = await Get(tokensToProcess);
            var verified = Validate(unverified);

            var openTrading = ProcessOpenTrading(verified[openTradingStr]);
            var addLiquidity = ProcessAddLiquidity(verified[addLiquidityStr]);

            var openTradingReceipts = await GetTransactionReceipts(openTrading, openTradingStr);
            var addLiquidityReceipts = await GetTransactionReceipts(addLiquidity, addLiquidityStr);

            var toUpdate =
                openTradingReceipts.
                Concat(addLiquidityReceipts).
                ToList();

            await SaveToDB_update(toUpdate);

            List<string> toDelete = new();

            foreach (var item in tokensToProcess)
            {
                if (!toUpdate.Any(x => x.tokenAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase)))
                {
                    toDelete.Add(item.contractAddress);
                }
            }

            await SaveToDB_delete(toDelete);
        }

        private async Task<List<(string tokenAddress, string pairAddress, string functionName, string blockNumber)>> GetTransactionReceipts(
            List<(string tokenAddress, string hashPair, string blockNumber)> tokensToProcess, string functionName)
        {

            List<(string, string, string, string)> res = new();

            var diff = tokensToProcess.Count();
            var items = tokensToProcess.Select(x => x.hashPair).ToList();

            Func<List<string>, int, Task<List<getTransactionReceiptDTO>>> apiMethod = apiAlchemy.getTransactionReceiptBatch;

            var t = await apiAlchemy.executeBatchCall(items, apiMethod, diff);

            foreach (var item in t)
            {
                if (item.result is null)
                {
                    continue;
                }

                var token = tokensToProcess.Where(x => x.hashPair == item.result.transactionHash).FirstOrDefault();

                foreach (var log in item.result.logs)
                {
                    var pairCreated = log.topics.Any(x => x.Contains("0x0000000000000000000000000000000000000000000000000000000000000000"));

                    if (pairCreated)
                    {
                        res.Add((token.tokenAddress, log.address, functionName, token.blockNumber));
                        break;
                    }
                }
            }

            return res;
        }

        private async Task<int> SaveToDB_update(
            List<(string tokenAddress, string pairAddress, string functionName, string blockNumber)> collection)
        {
            var res = 0;
            var ids = collection.Select(x => x.tokenAddress).ToList();
            var ethTrainDataToUpdate = await dbContext.EthTrainData.Where(x => ids.Contains(x.contractAddress)).ToListAsync();

            foreach (var item in ethTrainDataToUpdate)
            {
                var t = collection.Where(x => x.tokenAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                item.pairAddress = t.pairAddress;
                item.pairBlockNumberInt = Convert.ToInt32(t.blockNumber);
                item.pairAddressFunctionName = t.functionName;
            }

            res = await dbContext.SaveChangesAsync();

            return res;
        }

        private async Task<int> SaveToDB_delete(
            List<string> collection)
        {
            var res = 0;
            var ethTrainDataToDelete = await
                dbContext.
                EthTrainData.
                Where(x => collection.Contains(x.contractAddress) && (lastEthBlockNumber - x.blockNumberInt) > 20000).
                ToListAsync();

            foreach (var item in ethTrainDataToDelete)
            {
                item.pairAddress = "no";
            }

            dbContext.EthTrainData.UpdateRange(ethTrainDataToDelete);

            res = await dbContext.SaveChangesAsync();

            return res;
        }

        public List<(string tokenAddress, string hashPair, string blockNumber)> ProcessAddLiquidity(
            List<GetNormalTxnDTO.Result> collection)
        {

            List<(string, string, string)> res = new();

            foreach (var item in collection)
            {
                if (item.input is not null &&
                    item.functionName is not null)
                {
                    var tokenAddress = ApiWeb3.DecodeLiquidityInput(item.functionName, item.input);

                    if (!string.IsNullOrEmpty(tokenAddress))
                    {
                        res.Add((tokenAddress, item.hash, item.blockNumber));
                    }
                }
            }

            return res;
        }
        public List<(string tokenAddress, string hashPair, string blockNumber)> ProcessOpenTrading(
            List<GetNormalTxnDTO.Result> collection)
        {
            List<(string, string, string)> res = new();

            foreach (var item in collection)
            {
                if (item.to is not null)
                {
                    res.Add((item.to, item.hash, item.blockNumber));
                }
            }

            return res;
        }


        public async Task<List<GetNormalTxnDTO>> Get(
            List<EthTrainData> tokensToProcess)
        {
            List<GetNormalTxnDTO> res = new();

            var owners = tokensToProcess.Select(x => (x.from, x.blockNumberInt.ToString())).Distinct().ToList();

            res = await etherscanApi.getNormalTxnBatchRequest(owners);

            return res;
        }

        public Dictionary<string, List<GetNormalTxnDTO.Result>> Validate(
            List<GetNormalTxnDTO> collection)
        {
            Dictionary<string, List<GetNormalTxnDTO.Result>> res = new();
            List<GetNormalTxnDTO.Result> resO = new();
            List<GetNormalTxnDTO.Result> resA = new();

            foreach (var item in collection)
            {
                if (item.result is null)
                {
                    continue;
                }

                foreach (var t in item.result)
                {
                    if (t.functionName.Contains("openTrading", StringComparison.InvariantCultureIgnoreCase))
                    {
                        resO.Add(t);
                    }

                    if (t.functionName.Contains("addLiquidity", StringComparison.InvariantCultureIgnoreCase))
                    {
                        resA.Add(t);
                    }
                }
            }

            res.Add("openTrading", resO);
            res.Add("addLiquidity", resA);

            return res;
        }

        public async Task<List<EthTrainData>> GetTokensToProcess()
        {
            var res = await
                dbContext.
                EthTrainData.
                Where(x => string.IsNullOrEmpty(x.pairAddress) &&
                      x.walletCreated != default &&
                      x.walletCreated != default(DateTime).AddDays(1)).
                Take(1000).
                ToListAsync();

            return res;
        }
    }
}
