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
    public class IsDeadBySwaps
    {
        private readonly ILogger logger;
        private readonly ApiWeb3 ApiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        private readonly EtherscanApi etherscanApi;

        int lastEthBlockNumber = 0;
        public IsDeadBySwaps(
            ApiWeb3 ApiWeb3,
            EthApi apiAlchemy,
            dbContext dbContext,
            ILogger<IsDeadBySwaps> logger,
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

            var tokensToProcess = await GetTokensToProcess();

            await SaveToDB_update(tokensToProcess);
        }

        private async Task<int> SaveToDB_update(
             List<int?> collection)
        {
            var res = 0;
            var ethTrainDataToUpdate = await dbContext.EthTrainData.Where(x => collection.Contains(x.Id)).ToListAsync();

            foreach (var item in ethTrainDataToUpdate)
            {             
                item.isDead = true;
                item.DeadBlockNumber = lastEthBlockNumber;
            }

            res = await dbContext.SaveChangesAsync();

            return res;
        }

        public async Task<List<int?>> GetTokensToProcess()
        {
            List<int?> res = [];

            var ww = dbContext.EthTrainData.Where(x => x.isDead == false).Select(x => x.Id).ToList();

            res = await dbContext.
                EthSwapEvents.
                Where(x => x.EthTrainDataId != null && ww.Contains((int)x.EthTrainDataId)).
                GroupBy(x => x.EthTrainDataId).
                Where(x => (lastEthBlockNumber - x.Max(y => y.blockNumberInt)) > 144000).
                Select(x => x.Key).
                ToListAsync();

            return res;
        }
    }
}
