using api_alchemy.Eth;

using Data;
using Data.Models;

using etherscan.ResponseDTO;
using etherscan;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using nethereum;

namespace eth_shared
{
    public class IsDead
    {
        private readonly ILogger logger;
        private readonly ApiWeb3 ApiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        private readonly EtherscanApi etherscanApi;
        public IsDead(
            ApiWeb3 ApiWeb3,
            EthApi apiAlchemy,
            dbContext dbContext,
            ILogger<IsDead> logger,
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
            var tokensToProcess = await GetTokensToProcess();

            var unverified = await Get(tokensToProcess);
            var validated = Validate(unverified);
            var processed = ProcessLiquidity(validated);

            var toUpdate = processed;

            await SaveToDB_update(toUpdate);
            await SaveToDB_delete(tokensToProcess);
        }
        private async Task<int> SaveToDB_delete(
           List<EthTrainData> collection)
        {
            foreach (var item in collection)
            {
                item.isDeadInt = item.isDeadInt + 1;
            }

            dbContext.EthTrainData.UpdateRange(collection);
            var res = await dbContext.SaveChangesAsync();

            return res;
        }
        private async Task<int> SaveToDB_update(
             List<(string tokenAddress, int blockNumber)> collection)
        {
            var res = 0;
            var ids = collection.Select(x => x.tokenAddress).ToList();
            var ethTrainDataToUpdate = await dbContext.EthTrainData.Where(x => ids.Contains(x.contractAddress)).ToListAsync();

            foreach (var item in ethTrainDataToUpdate)
            {
                var t = collection.Where(x => x.tokenAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                item.isDead = true;
                item.DeadBlockNumber = t.blockNumber;
            }

            res = await dbContext.SaveChangesAsync();

            return res;
        }
        public List<(string tokenAddress, int blockNumber)> ProcessLiquidity(
            List<GetNormalTxnDTO.Result> collection)
        {

            List<(string, int)> res = new();

            foreach (var item in collection)
            {
                if (item.input is not null &&
                    item.functionName is not null)
                {
                    var tokenAddress = ApiWeb3.DecodeLiquidityInput(item.functionName, item.input);

                    if (!string.IsNullOrEmpty(tokenAddress))
                    {
                        var blockNumberInt = Convert.ToInt32(item.blockNumber);
                        res.Add((tokenAddress.ToLower(), blockNumberInt));
                    }
                }
            }

            return res;
        }

        public List<GetNormalTxnDTO.Result> Validate(
             List<GetNormalTxnDTO> collection)
        {
            List<GetNormalTxnDTO.Result> res = new();

            foreach (var item in collection)
            {
                if (item.result is null)
                {
                    continue;
                }

                foreach (var t in item.result)
                {
                    if (t.functionName.Contains("removeLiquidity", StringComparison.InvariantCultureIgnoreCase))
                    {
                        res.Add(t);
                    }
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

        public async Task<List<EthTrainData>> GetTokensToProcess()
        {
            var res = await
                dbContext.
                EthTrainData.
                Where(x => x.isDead == false).
                OrderBy(x => x.isDeadInt).
                Take(100).
                ToListAsync();

            return res;
        }
    }
}
