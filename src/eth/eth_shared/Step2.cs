using Data;
using Data.Models;

using etherscan;

using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public class Step2
    {
        private List<EthTrainData> ethTrainDatas = new();

        private readonly ILogger logger;
        private readonly GetPair getPair;
        private readonly dbContext dbContext;
        private readonly EtherscanApi etherscanApi;
        private readonly GetWalletAge getWalletAge;
        private readonly GetSourceCode getSourceCode;

        public Step2(
            ILogger<Step2> logger,
            GetPair getPair,
            dbContext dbContext,
            EtherscanApi etherscanApi,
            GetWalletAge getWalletAge,
            GetSourceCode getSourceCode
            )
        {
            this.logger = logger;
            this.getPair = getPair;
            this.dbContext = dbContext;
            this.getWalletAge = getWalletAge;
            this.etherscanApi = etherscanApi;
            this.getSourceCode = getSourceCode;
        }
        public async Task Start()
        {
            await getSourceCode.Start();
            await getPair.Start();
            await getWalletAge.Start();
        }

    }
}
