using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Shared.BaseScan.Model;
using Shared.ConfigurationOptions;
using Shared.DB;

namespace Shared.BaseScan
{
    public class BaseScanContractScraper
    {
        private readonly DBContext dBContext;
        private readonly BaseScanApiClient baseScan;
        private readonly BaseScanApiClient baseScanPayedSubscription;
        private readonly OptionsBaseScan optionsBaseScan;
        public BaseScanContractScraper(
            IOptions<OptionsBaseScan> optionsBaseScan,
            DBContext dBContext,
            BaseScanApiClient baseScan,
            BaseScanApiClient baseScanPayedSubscription)
        {
            this.optionsBaseScan = optionsBaseScan.Value;
            this.dBContext = dBContext;

            this.baseScan = baseScan;
            baseScan.SetApiKeyToken(1);

            this.baseScanPayedSubscription = baseScanPayedSubscription;
            baseScanPayedSubscription.SetApiKeyToken(3);
        }

        public async Task<int> Start()
        {
            var res = 0;

            var lastBlockNumber = await GetLastProcessedBlockFromDB();
            var lastBlockNumber16 = (lastBlockNumber + 1).ToString("X");
            var transIntheBlock = await baseScan.GetBlockByNumber(lastBlockNumber16);

            if (transIntheBlock.result is null)
            {
                return res;
            }

            var contracts = await FindContracts(transIntheBlock);


            if (contracts.Count > 0)
            {
                foreach (var item in contracts)
                {
                    NormalTransactions listOfNormalTransactions = new();

                    for (int i = 0; i < 3; i++)
                    {
                        listOfNormalTransactions = await baseScan.GetListOfNormalTransactions(item.from);
                        var tokenAddress = await FindTokenAddress(listOfNormalTransactions, item.hash);
                        item.contractAddress = tokenAddress;

                        if (!string.IsNullOrEmpty(item.contractAddress) || listOfNormalTransactions?.result?.Count() == 999)
                        {
                            break;
                        }

                        await Task.Delay(2000);
                    }
                }

                res = await SaveNewContractsToDB(contracts);
            }

            if (contracts.Count == 0 || res == 0)
            {
                res = await UpdateLastBlockWhenNoContractsToDB(lastBlockNumber);
            }

            return res;
        }
        private async Task<Model.TokenInfo> GetFromPaidApi(string contractAddress)
        {
            Model.TokenInfo res = new() { status = "0", message = "NOTOK" };

            try
            {
                res = await baseScanPayedSubscription.GetTokenInfo(contractAddress);

            }
            catch (Exception ex)
            {

            }

            return res;
        }

        private async Task<int> GetLastProcessedBlockFromDB()
        {
            var res = 0;

            res = await dBContext.TokenInfos.MaxAsync(x => x.BlockNumber);
            //res = 14246802;

            return res;
        }

        private async Task<string> FindTokenAddress(
            NormalTransactions listOfNormalTransactions,
            string transHash)
        {
            var res = "";

            if (listOfNormalTransactions.result is null)
            {
                return res;
            }

            foreach (var item in listOfNormalTransactions.result)
            {
                if (item.hash == transHash &&
                    !string.IsNullOrEmpty(item.contractAddress))
                {
                    res = item.contractAddress;
                    break;
                }
            }

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
        public async Task UpdateDBWithPaidApiTokenInfo(DB.TokenInfo tokenInfo)
        {
            if (!string.IsNullOrEmpty(tokenInfo.AddressToken))
            {
                var additional = await GetFromPaidApi(tokenInfo.AddressToken);

                if (additional.status.Equals("1") && additional.result is not null && additional.result.Count() == 1)
                {
                    var t = tokenInfo;

                    var additionalResult = additional.result[0];
                    t.NameToken = additionalResult.tokenName;
                    t.symbol = additionalResult.symbol;
                    t.divisor = additionalResult.divisor;
                    t.tokenType = additionalResult.tokenType;
                    t.totalSupply = additionalResult.totalSupply;
                    t.blueCheckmark = additionalResult.blueCheckmark;
                    t.description = additionalResult.description;
                    t.website = additionalResult.website;
                    t.email = additionalResult.email;
                    t.blog = additionalResult.blog;
                    t.reddit = additionalResult.reddit;
                    t.slack = additionalResult.slack;
                    t.facebook = additionalResult.facebook;
                    t.twitter = additionalResult.twitter;
                    t.bitcointalk = additionalResult.bitcointalk;
                    t.github = additionalResult.github;
                    t.telegram = additionalResult.telegram;
                    t.linkedin = additionalResult.linkedin;
                    t.discord = additionalResult.discord;
                    t.wechat = additionalResult.wechat;
                    t.whitepaper = additionalResult.whitepaper;
                    t.tokenPriceUSD = additionalResult.tokenPriceUSD;
                }
            }
        }

        private async Task<int> SaveNewContractsToDB(List<BlockByNumberModel.Transaction> collection)
        {
            var res = 0;

            var ti = new List<DB.TokenInfo>();

            foreach (var item in collection)
            {
                var countOfRecordsWithOwnerAddress =
                    await dBContext.
                    TokenInfos.
                    Where(x => x.AddressOwnersWallet == item.from).
                    CountAsync();

                if (countOfRecordsWithOwnerAddress >= 5)
                {
                    continue;
                }

                var t = new DB.TokenInfo();

                t.HashContractTransaction = item.hash;
                t.AddressToken = item.contractAddress;
                t.AddressOwnersWallet = item.from;

                t.BlockNumber = Convert.ToInt32(item.blockNumber, 16);

                t.UrlToken = $"{optionsBaseScan.UrlBasescanOrgToken}{t.AddressToken}";
                t.UrlOwnersWallet = $"{optionsBaseScan.UrlBasescanOrgAddress}{t.AddressOwnersWallet}";
                t.UrlChart = $"{optionsBaseScan.UrlDexscreenerComBase}{t.AddressToken}";

                t.TimeAdded = DateTime.UtcNow;
                t.TimeUpdated = DateTime.UtcNow;

                await UpdateDBWithPaidApiTokenInfo(t);

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
