using api_tokenSniffer;

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
        private readonly IsDead isDead;
        private readonly GetPair getPair;
        private readonly dbContext dbContext;
        private readonly tlgrmApi.tlgrmApi tlgrmApi;
        private readonly EtherscanApi etherscanApi;
        private readonly GetWalletAge getWalletAge;
        private readonly GetSourceCode getSourceCode;
        private readonly GetSwapEvents getSwapEvents;
        private readonly GetTokenSniffer getTokenSniffer;
        private readonly GetReservesLogs getReservesLogs;
        private readonly GetBalanceOnCreating getBalanceOnCreating;

        public Step2(
            ILogger<Step2> logger,
            IsDead isDead,
            GetPair getPair,
            dbContext dbContext,
            EtherscanApi etherscanApi,
            GetWalletAge getWalletAge,
            tlgrmApi.tlgrmApi tlgrmApi,
            GetSourceCode getSourceCode,
            GetSwapEvents getSwapEvents,
            GetTokenSniffer getTokenSniffer,
            GetReservesLogs getReservesLogs,
            GetBalanceOnCreating getBalanceOnCreating
            )
        {
            this.logger = logger;
            this.isDead = isDead;
            this.getPair = getPair;
            this.tlgrmApi = tlgrmApi;
            this.dbContext = dbContext;
            this.getWalletAge = getWalletAge;
            this.etherscanApi = etherscanApi;
            this.getSourceCode = getSourceCode;
            this.getSwapEvents = getSwapEvents;
            this.getReservesLogs = getReservesLogs;
            this.getTokenSniffer = getTokenSniffer;
            this.getBalanceOnCreating = getBalanceOnCreating;
        }
        public async Task Start()
        {
            //await getTokenSniffer.Start();
            //await getReservesLogs.Start();

            await getSwapEvents.Start();
            await isDead.Start();
            await getBalanceOnCreating.Start();
            await getSourceCode.Start();
            await getWalletAge.Start();
            await getPair.Start();
            await SendTlgrmMessageP0();
            await SendTlgrmMessageP10();
        }

        public async Task SendTlgrmMessageP0()
        {
            var notDefault = default(DateTime).AddDays(1);
            var ethTrainData =
                dbContext.
                EthTrainData.
                //Where(x=>x.contractAddress == "0x3f4e95bf39bc676c4f7eaccd4d2d353fa2891190").
                //Where(
                //    x => x.walletCreated > notDefault &&
                //    x.BalanceOnCreating >= 0).
                //Take(2).
                Where(
                    x => x.walletCreated > notDefault &&
                    x.ABI != "no" &&
                    x.BalanceOnCreating >= 0 &&
                    x.tlgrmNewTokens == 0 &&
                    x.isDead == false &&
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
                    x.pairAddress != "no" &&
                    x.tlgrmLivePairs == 0 &&
                    x.isDead == false &&
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
                    item.tlgrmLivePairs = resp.tlgrmMsgId;
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
