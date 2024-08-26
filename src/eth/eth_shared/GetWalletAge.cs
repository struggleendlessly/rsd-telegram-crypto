using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using etherscan;
using etherscan.ResponseDTO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public class GetWalletAge
    {
        private readonly ILogger logger;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        private readonly EtherscanApi etherscanApi;

        int lastEthBlockNumber = 0;

        public GetWalletAge(
            EthApi apiAlchemy,
            dbContext dbContext,
            EtherscanApi etherscanApi,
            ILogger<GetWalletAge> logger
            )
        {
            this.logger = logger;
            this.apiAlchemy = apiAlchemy;
            this.dbContext = dbContext;
            this.etherscanApi = etherscanApi;
        }

        public async Task Start()
        {
            lastEthBlockNumber = await apiAlchemy.lastBlockNumber();

            var tokensToProcess = await GetTokensToProcess();
            var unverified = await Get(tokensToProcess);
            var verified = Validate(unverified);

            var ids = verified.Select(x => x.to).ToList();
            var toUpdate = tokensToProcess.Where(x => ids.Contains(x.from)).ToList();

            var processedUpdate = await ProcessUpdate(toUpdate, verified);

            var walletNames = await dbContext.WalletNames.ToListAsync();

            var getNormalTxnForWalletSources = await GetNormalTxn(toUpdate, walletNames);
            var validateForRemoveLiquidityOnSource = await ValidateForRemoveLiquidity(getNormalTxnForWalletSources, toUpdate);

            var updated = await SaveToDB_update(validateForRemoveLiquidityOnSource);

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

        public async Task<List<EthTrainData>> ValidateForRemoveLiquidity(
            List<GetNormalTxnDTO> getNormalTxnDTOs,
            List<EthTrainData> ethTrainDatas)
        {
            foreach (var item in getNormalTxnDTOs)
            {
                if (item.result is not null &&
                    item.result.Count() > 0)
                {
                    var t = item.result.Where(x => x.functionName.Contains("removeLiquidity", StringComparison.InvariantCultureIgnoreCase)).Count();
                    var td = ethTrainDatas.Where(x => x.WalletSource1in.Equals(item.ownerAddresses, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                    if (td is not null)
                    {
                        td.WalletSource1inCountRemLiq = t;
                        td.isDead = true;
                        td.DeadBlockNumber = td.blockNumberInt;
                    }
                }
            }

            return ethTrainDatas;
        }

        public async Task<List<GetNormalTxnDTO>> GetNormalTxn(
             List<EthTrainData> tokensToProcess,
             List<WalletNames> walletNames)
        {
            List<GetNormalTxnDTO> res = new();

            var owners =
                tokensToProcess.
                Where(x => !walletNames.Any(v => v.Address.Equals(x.WalletSource1in, StringComparison.InvariantCultureIgnoreCase))).
                Select(x => (x.WalletSource1in, "0")).
                Distinct().
                ToList();

            res = await etherscanApi.getNormalTxnBatchRequest(owners);

            return res;
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
                var t = getAssetTransfersDTOs.Where(x => x.to == item.from).FirstOrDefault();

                if (t is not null)
                {
                    item.walletCreated = t.metadata.blockTimestamp;
                    item.WalletSource1in = t.from;
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
                Where(x => x.walletCreated == default || x.walletCreated == default(DateTime).AddDays(1)).
                Take(200).
                ToListAsync();

            return res;
        }
    }
}
