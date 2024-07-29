using Data;
using Data.Models;

using etherscan;

namespace eth_shared
{
    public class Step2
    {
        private List<EthTrainData> ethTrainDatas = new();

        private readonly EtherscanApi etherscanApi;
        private readonly dbContext dbContext;
        private readonly GetSourceCode getSourceCode;

        public Step2(
            EtherscanApi etherscanApi,
            dbContext dbContext,
            GetSourceCode getSourceCode
            )
        {
            this.dbContext = dbContext;
            this.etherscanApi = etherscanApi;
            this.getSourceCode = getSourceCode;
        }
        public async Task Start()
        {
            await getSourceCode.Start();
        }

    }
}
