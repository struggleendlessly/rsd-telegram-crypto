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

            var notDefault = default(DateTime).AddDays(1);
            var mes = dbContext.EthTrainData.Where(x => x.walletCreated > notDefault && x.BalanceOnCreating>=0).Take(2).ToList();
            var ids = mes.Select(x => x.blockNumberInt).ToList();
            var blocks = dbContext.EthBlock.Where(x => ids.Contains(x.numberInt)).ToList();
            await tlgrmApi.SendPO(mes, blocks);
        }

    }
}
