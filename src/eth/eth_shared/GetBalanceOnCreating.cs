using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using eth_shared.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Nethereum.Hex.HexTypes;

using System.Globalization;
using System.Numerics;

namespace eth_shared
{
    public class GetBalanceOnCreating
    {
        private readonly ILogger logger;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;

        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        string decimalCeparator = ",";
        public GetBalanceOnCreating(
             EthApi apiAlchemy,
             dbContext dbContext,
             ILogger<GetBalanceOnCreating> logger
            )
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;

            decimalCeparator = currentCulture.NumberFormat.NumberDecimalSeparator;
        }

        public async Task Start()
        {
            var tokensToProcess = await GetTokensToProcess();
            var unverified = await Get(tokensToProcess);
            var verified = Validate(unverified);

            var ids = verified.Select(x => x.id).ToList();
            var toUpdate = tokensToProcess.Where(x => ids.Contains(x.Id)).ToList();

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

        public async Task<List<EthTrainData>> ProcessDelete(
            List<EthTrainData> toUpdate)
        {
            List<EthTrainData> res = new();

            foreach (var item in toUpdate)
            {
                item.BalanceOnCreating = -2;
                res.Add(item);
            }

            return res;
        }
        public async Task<List<EthTrainData>> ProcessUpdate(
            List<EthTrainData> toUpdate,
            List<getBalance> getBalanceDTOs
            )
        {
            List<EthTrainData> res = new();

            try
            {

                foreach (var item in toUpdate)
                {
                    var t = getBalanceDTOs.Where(x => x.id == item.Id).FirstOrDefault();

                    if (t is not null)
                    {
                        BigInteger balanceBI = 0;

                        balanceBI = new HexBigInteger(t.result).Value;
                        var balanceStrFull = balanceBI.ToString().FormatTo18(decimalCeparator);
                        var balanceStrShort = balanceStrFull.GetFirstThreeAfterComma(decimalCeparator);

                        var balance = 0.0;

                        balance = double.Parse(balanceStrShort, NumberStyles.Any, currentCulture);

                        item.BalanceOnCreating = balance;

                        res.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

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

        private List<getBalance> Validate(
              List<getBalance> unverified)
        {
            List<getBalance> res = new();

            foreach (var item in unverified)
            {
                if (!string.IsNullOrEmpty(item.result))
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public async Task<List<getBalance>> Get(
            List<EthTrainData> ethTrainDatas)
        {
            List<getBalance> res = new();

            var diff = ethTrainDatas.Count();
            var items = ethTrainDatas;

            Func<List<EthTrainData>, int, Task<List<getBalance>>> apiMethod = apiAlchemy.getBalance;

            res = await apiAlchemy.executeBatchCall(items, apiMethod, diff);

            return res;
        }

        public async Task<List<EthTrainData>> GetTokensToProcess()
        {
            var res = await
                dbContext.
                EthTrainData.
                Where(x => x.BalanceOnCreating == -1).
                Take(100).
                ToListAsync();

            return res;
        }
    }
}
