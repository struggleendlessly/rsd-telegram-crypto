using Data;
using Data.Models;

using etherscan;

using Microsoft.Extensions.Logging;

using tlgrmApi;



namespace eth_shared
{
    public class Step2
    {
        private List<EthTrainData> ethTrainDatas = new();

        private readonly ILogger logger;
        private readonly GetPair getPair;
        private readonly dbContext dbContext;
        private readonly tlgrmApi.tlgrmApi tlgrmApi;
        private readonly EtherscanApi etherscanApi;
        private readonly GetWalletAge getWalletAge;
        private readonly GetSourceCode getSourceCode;
        private readonly GetBalanceOnCreating getBalanceOnCreating;

        public Step2(
            ILogger<Step2> logger,
            GetPair getPair,
            tlgrmApi.tlgrmApi tlgrmApi,
            dbContext dbContext,
            EtherscanApi etherscanApi,
            GetWalletAge getWalletAge,
            GetSourceCode getSourceCode,
            GetBalanceOnCreating getBalanceOnCreating
            )
        {
            this.logger = logger;
            this.getPair = getPair;
            this.dbContext = dbContext;
            this.getWalletAge = getWalletAge;
            this.etherscanApi = etherscanApi;
            this.getSourceCode = getSourceCode;
            this.getBalanceOnCreating = getBalanceOnCreating;
            this.tlgrmApi = tlgrmApi;
        }
        public async Task Start()
        {
            //await getBalanceOnCreating.Start();
            //await getSourceCode.Start();
            //await getPair.Start();
            //await getWalletAge.Start();
            //await SendTlgrmMessageP0();
            await SendTlgrmMessageP10();
        }

        public async Task SendTlgrmMessageP0()
        {
            var notDefault = default(DateTime).AddDays(1);
            var ethTrainData =
                dbContext.
                EthTrainData.
                //Where(
                //    x => x.walletCreated > notDefault &&
                //    x.BalanceOnCreating >= 0).
                //Take(2).
                Where(
                    x => x.walletCreated > notDefault &&
                    x.ABI != "no" &&
                    x.BalanceOnCreating >= 0 &&
                    x.tlgrmNewTokens == 0 &&
                    x.blockNumberInt > 20420936).
                ToList();

            var ids = ethTrainData.Select(x => x.blockNumberInt).ToList();
            var blocks = dbContext.EthBlock.Where(x => ids.Contains(x.numberInt)).ToList();

            var t = await tlgrmApi.SendPO(ethTrainData, blocks);

            foreach (var item in ethTrainData)
            {
                var resp = t.FirstOrDefault(x => x.contractAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase));

                if (resp is not null)
                {
                    item.tlgrmNewTokens = resp.tlgrmMsgId;
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task SendTlgrmMessageP10()
        {
            var notDefault = default(DateTime).AddDays(1);
            var ethTrainData =
                dbContext.
                EthTrainData.
                //Where(
                //    x => x.walletCreated > notDefault &&
                //    x.BalanceOnCreating >= 0).
                //Take(2).
                Where(
                    x =>
                    x.pairAddress != "" &&
                    x.tlgrmLivePairs == 0 &&
                    x.blockNumberInt > 20456589).
                ToList();

            var ids = ethTrainData.Select(x => x.blockNumberInt).ToList();
            var blocks = dbContext.EthBlock.Where(x => ids.Contains(x.numberInt)).ToList();

            var t = await tlgrmApi.SendP1O(ethTrainData, blocks);

            foreach (var item in ethTrainData)
            {
                var resp = t.FirstOrDefault(x => x.contractAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase));

                if (resp is not null)
                {
                    item.tlgrmNewTokens = resp.tlgrmMsgId;
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
