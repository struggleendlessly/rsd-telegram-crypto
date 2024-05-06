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
        private readonly BaseScan.BaseScanApiClient baseScan;
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
            var transIntheBlock = await baseScan.GetBlockByNumber(lastBlockNumber);
            var contracts = await FindContracts(transIntheBlock);
            res = await SaveNewContractsToDB(contracts);

            return res;
        }

        private async Task<string> GetLastProcessedBlockFromDB()
        {
            var res = string.Empty;

            var lastBlockNumber = await dBContext.TokenInfos.MaxAsync(x => x.BlockNumber);
            res = lastBlockNumber.ToString("X");

            return res;
        }

        private async Task<List<BlockByNumberModel.Transaction>> FindContracts(BlockByNumberModel collection)
        {
            var res = new List<BlockByNumberModel.Transaction>();

            foreach (var item in collection.result.transactions)
            {
                if (string.IsNullOrEmpty(item.to))
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
                t.BlockNumber = int.Parse(item.blockNumber, System.Globalization.NumberStyles.HexNumber);

                t.UrlChart = $"{optionsBaseScan.UrlDexscreenerComBase}{t.AddressToken}";
                t.UrlChart = $"{optionsBaseScan.UrlBasescanOrgAddress}{t.AddressOwnersWallet}";

                ti.Add(t);
            }

            await dBContext.TokenInfos.AddRangeAsync(ti);
            res = await dBContext.SaveChangesAsync();

            return res;
        }
    }
}
