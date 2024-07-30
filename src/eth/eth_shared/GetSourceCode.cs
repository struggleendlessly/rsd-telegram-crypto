using api_alchemy.Eth;

using Data;
using Data.Models;

using eth_shared.Map;

using etherscan;
using etherscan.ResponseDTO;

using Microsoft.EntityFrameworkCore;

namespace eth_shared
{
    public class GetSourceCode
    {
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;
        private readonly EtherscanApi etherscanApi;
        public GetSourceCode(
             EthApi apiAlchemy,
             dbContext dbContext,
             EtherscanApi etherscanApi
            )
        {
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
            this.etherscanApi = etherscanApi;
        }

        public async Task Start()
        {
            var tokensToProcess = await GetTokensToProcess();
            var unverified = await Get(tokensToProcess);
            var verified = Validate(unverified);
            var filtered = Fiter(verified);

            var toDelete = unverified.Except(filtered).ToList();

            await SaveToDB_new(filtered);
            await SaveToDB_delete(toDelete);

        }

        public async Task<List<GetSourceCodeDTO>> Get(List<EthTrainData> tokensToProcess)
        {
            var res = await etherscanApi.getSourceCodeBatchRequest(tokensToProcess.Select(x => x.contractAddress).ToList());
            return res;
        }

        private async Task<int> SaveToDB_new(List<GetSourceCodeDTO> collection)
        {
            var res = 0;
            var ids = collection.Select(x => x.contractAddress).ToList();
            var ethTrainDataToUpdate = await dbContext.EthTrainData.Where(x => ids.Contains(x.contractAddress)).ToListAsync();

            foreach (var item in ethTrainDataToUpdate)
            {
                var sourceCode = collection.Where(x => x.contractAddress == item.contractAddress).FirstOrDefault();

                if (sourceCode is not null)
                {
                    item.Map(sourceCode);
                }
            }

            res = await dbContext.SaveChangesAsync();

            return res;
        }

        private async Task<int> SaveToDB_delete(List<GetSourceCodeDTO> collection)
        {
            var res = 0;
            var ids = collection.Select(x => x.contractAddress).ToList();
            var ethTrainDataToDelete = await dbContext.EthTrainData.Where(x => ids.Contains(x.contractAddress)).ToListAsync();

            dbContext.EthTrainData.RemoveRange(ethTrainDataToDelete);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        public List<GetSourceCodeDTO> ToDelete(List<GetSourceCodeDTO> collection)
        {
            List<GetSourceCodeDTO> res = new();

            foreach (var item in collection)
            {
                var sourceCode = item.result.FirstOrDefault();

                if (item is not null &&
                    item.result is not null &&
                    item.result.Count == 1 &&
                    sourceCode is not null &&
                    !string.IsNullOrEmpty(sourceCode.SourceCode) &&
                    !string.IsNullOrEmpty(sourceCode.ABI)
                    )
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public List<GetSourceCodeDTO> Validate(List<GetSourceCodeDTO> collection)
        {
            List<GetSourceCodeDTO> res = new();

            foreach (var item in collection)
            {
                var sourceCode = item.result.FirstOrDefault();

                if (item is not null &&
                    item.result is not null &&
                    item.result.Count == 1 &&
                    sourceCode is not null &&
                    !string.IsNullOrEmpty(sourceCode.SourceCode) &&
                    !string.IsNullOrEmpty(sourceCode.ABI)
                    )
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public List<GetSourceCodeDTO> Fiter(List<GetSourceCodeDTO> collection)
        {
            List<GetSourceCodeDTO> res = new();

            foreach (var item in collection)
            {
                var sourceCode = item.result.FirstOrDefault();

                if (item is not null &&
                    item.result is not null &&
                    item.result.Count == 1 &&
                    sourceCode is not null &&
                    !sourceCode.SourceCode.Contains("addbot", StringComparison.InvariantCultureIgnoreCase) &&
                    !sourceCode.SourceCode.Contains("addb0t", StringComparison.InvariantCultureIgnoreCase) &&
                    !sourceCode.SourceCode.Contains("addbots", StringComparison.InvariantCultureIgnoreCase) &&
                    !sourceCode.SourceCode.Contains("addb0ts", StringComparison.InvariantCultureIgnoreCase)
                    )
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public async Task<List<EthTrainData>> GetTokensToProcess()
        {
            var lastEthBlockNumber = await apiAlchemy.lastBlockNumber();
            var diff = 8000;

            var res = await
                dbContext.
                EthTrainData.
                Where(x => string.IsNullOrEmpty(x.ABI) && x.blockNumberInt < (lastEthBlockNumber - diff)).
                Take(100).
                ToListAsync();

            return res;
        }
    }
}
