using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Shared.BaseScan.Model;
using Shared.ConfigurationOptions;
using Shared.DB;

namespace Shared.BaseScan
{
    public class BaseScanContractScraper
    {
        private readonly DBContext dBContext;
        private readonly BaseScanApiClient baseScan;
        private readonly OptionsBaseScan optionsBaseScan;
        public BaseScanContractScraper(
            IOptions<OptionsBaseScan> optionsBaseScan,
            DBContext dBContext,
            BaseScanApiClient baseScan)
        {
            this.optionsBaseScan = optionsBaseScan.Value;
            this.dBContext = dBContext;
            this.baseScan = baseScan;
        }

        public async Task<int> Start()
        {
            var res = 0;

            var lastBlockNumber = await GetLastProcessedBlockFromDB();
            var lastBlockNumber16 = (lastBlockNumber + 1).ToString("X");
            var transIntheBlock = await baseScan.GetBlockByNumber(lastBlockNumber16);
            var contracts = await FindContracts(transIntheBlock);

            if (contracts.Count == 0)
            {
                res = await UpdateLastBlockWhenNoContractsToDB(lastBlockNumber);
            }
            else
            {
                res = await SaveNewContractsToDB(contracts);
            }

            return res;
        }

        private async Task<int> GetLastProcessedBlockFromDB()
        {
            var res = 0;

            res = await dBContext.TokenInfos.MaxAsync(x => x.BlockNumber);

            return res;
        }

        private async Task<List<BlockByNumberModel.Transaction>> FindContracts(BlockByNumberModel collection)
        {
            var res = new List<BlockByNumberModel.Transaction>();

            foreach (var item in collection.result.transactions)
            {
                if (string.IsNullOrEmpty(item.to) &&
                    item.input.Contains("0x60806040") &&
                    item.input.Length > 3000
                    )
                {
                    res.Add(item);
                }
            }

            return res;
        }

        private async Task<int> SaveNewContractsToDB(List<BlockByNumberModel.Transaction> collection)
        {
            var res = 0;

            var ti = new List<TokenInfo>();

            foreach (var item in collection)
            {
                var t = new TokenInfo();
                t.AddressToken = item.hash;
                t.AddressOwnersWallet = item.from;
                t.BlockNumber = Convert.ToInt32(item.blockNumber, 16);

                t.UrlChart = $"{optionsBaseScan.UrlDexscreenerComBase}{t.AddressToken}";
                t.UrlChart = $"{optionsBaseScan.UrlBasescanOrgAddress}{t.AddressOwnersWallet}";

                ti.Add(t);
            }

            await dBContext.TokenInfos.AddRangeAsync(ti);
            res = await dBContext.SaveChangesAsync();

            return res;
        }

        private async Task<int> UpdateLastBlockWhenNoContractsToDB(int lastBlockNumber)
        {
            var res = 0;

            var t = await dBContext.TokenInfos.Where(x => x.BlockNumber == lastBlockNumber).FirstOrDefaultAsync();

            if (t is not null)
            {
                t.BlockNumber = (lastBlockNumber + 1);
            }

            res = await dBContext.SaveChangesAsync();

            return res;
        }
    }
}
