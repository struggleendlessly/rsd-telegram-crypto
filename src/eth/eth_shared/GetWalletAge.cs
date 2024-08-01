using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public class GetWalletAge
    {
        private readonly ILogger logger;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;

        int lastEthBlockNumber = 0;

        public GetWalletAge(
            EthApi apiAlchemy,
            dbContext dbContext,
            ILogger<GetWalletAge> logger
            )
        {
            this.logger = logger;
            this.apiAlchemy = apiAlchemy;
            this.dbContext = dbContext;
        }

        public async Task Start()
        {
            lastEthBlockNumber = await apiAlchemy.lastBlockNumber();

            var tokensToProcess = await GetTokensToProcess();
            var unverified = await Get(tokensToProcess);
            var verified = Validate(unverified);

            var ids = verified.Select(x => x.from).ToList();
            var toUpdate = tokensToProcess.Where(x => ids.Contains(x.from)).ToList();

            var processedUpdate = await ProcessUpdate(toUpdate, verified);

            var updated = await SaveToDB_update(processedUpdate);

            List<EthTrainData> toDelete = new();

            foreach (var item in tokensToProcess)
            {
                if (!toUpdate.Any(x => x.contractAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase)))
                {
                    toDelete.Add(item);
                }
            }

            var processedDelete = await ProcessDelete(toDelete);
            var deleted = await SaveToDB_delete(processedDelete);
        }

        private async Task<int> SaveToDB_delete(List<EthTrainData> ethTrainDatas)
        {
            var res = 0;

            dbContext.EthTrainData.UpdateRange(ethTrainDatas);
            res = await dbContext.SaveChangesAsync();

            return res;
        }
        private async Task<int> SaveToDB_update(List<EthTrainData> ethTrainDatas)
        {
            var res = 0;

            dbContext.EthTrainData.UpdateRange(ethTrainDatas);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        public async Task<List<EthTrainData>> ProcessDelete(
            List<EthTrainData> toUpdate
            )
        {
            List<EthTrainData> res = new();

            foreach (var item in toUpdate)
            {
                item.walletCreated = item.walletCreated.AddDays(1);
                res.Add(item);
            }

            return res;
        }

        public async Task<List<EthTrainData>> ProcessUpdate(
            List<EthTrainData> toUpdate,
            List<getAssetTransfersDTO.Transfer> getAssetTransfersDTOs
            )
        {
            List<EthTrainData> res = new();

            foreach (var item in toUpdate)
            {
                var t = getAssetTransfersDTOs.Where(x => x.from == item.from).FirstOrDefault();

                if (t is not null)
                {
                    item.walletCreated = t.metadata.blockTimestamp;
                    res.Add(item);
                }
            }

            return res;
        }

        private List<getAssetTransfersDTO.Transfer> Validate(
            List<getAssetTransfersDTO> unverified)
        {
            List<getAssetTransfersDTO.Transfer> res = new();

            foreach (var item in unverified)
            {
                if (item.result is not null &&
                    item.result.transfers is not null &&
                    item.result.transfers.Count() > 0)
                {
                    res.Add(item.result.transfers.First());
                }
            }

            return res;
        }

        public async Task<List<getAssetTransfersDTO>> Get(
            List<EthTrainData> ethTrainDatas)
        {
            List<getAssetTransfersDTO> res = new();

            var diff = ethTrainDatas.Count();
            var items = ethTrainDatas;

            Func<List<EthTrainData>, int, Task<List<getAssetTransfersDTO>>> apiMethod = apiAlchemy.getAssetTransfers;

            res = await apiAlchemy.executeBatchCall(items, apiMethod, diff);

            return res;
        }

        public async Task<List<EthTrainData>> GetTokensToProcess()
        {
            var res = await
                dbContext.
                EthTrainData.
                Where(x => x.walletCreated == default).
                Take(30).
                ToListAsync();

            return res;
        }
    }
}
