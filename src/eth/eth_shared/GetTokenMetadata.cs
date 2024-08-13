using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Microsoft.Extensions.Logging;
using nethereum;

using System.Text.RegularExpressions;

namespace eth_shared
{
    public class GetTokenMetadata
    {
        private readonly ILogger logger;
        private readonly EthApi apiAlchemy;
        public GetTokenMetadata(
            ILogger<GetTokenMetadata> logger,
            EthApi apiAlchemy)
        {
            this.logger = logger;
            this.apiAlchemy = apiAlchemy;
        }

        public async Task<List<getTokenMetadataDTO>> Start(List<getTransactionReceiptDTO.Result> txnReceiptsFiltered)
        {
            Thread.Sleep(1000);

            var unfiltered = await Get(txnReceiptsFiltered);
            var validated = Validate(unfiltered);
            var res = Filter(validated);

            return res;
        }

        private async Task<List<getTokenMetadataDTO>> Get(List<getTransactionReceiptDTO.Result> txnReceiptsFiltered)
        {
            List<getTokenMetadataDTO> res = new();

            for (int i = 0; i < txnReceiptsFiltered.Count; i++)
            {
                var item = txnReceiptsFiltered[i];
                item.txnNumberForMetadata = i;
            }

            var diff = txnReceiptsFiltered.Count();
            var items = txnReceiptsFiltered;

            Func<List<getTransactionReceiptDTO.Result>, int, Task<List<getTokenMetadataDTO>>> apiMethod = apiAlchemy.getTokenMetadataBatch;

            res = await apiAlchemy.executeBatchCall(items, apiMethod, diff);

            return res;
        }

        public List<getTokenMetadataDTO> Validate(List<getTokenMetadataDTO> collection)
        {
            List<getTokenMetadataDTO> res = new();

            foreach (var item in collection)
            {
                if (item.result is not null)
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public List<getTokenMetadataDTO> Filter(List<getTokenMetadataDTO> collection)
        {
            List<getTokenMetadataDTO> res = new();

            foreach (var item in collection)
            {
                var name = item.result.name;
                var decimals = item.result.decimals;

                // English letters, numbers, spaces, '
                string pattern = @"^[A-Za-z0-9\s']+$";

                if (decimals is not null &&
                    name is not null &&
                    !name.Contains("test", StringComparison.OrdinalIgnoreCase) &&
                    Regex.IsMatch(name, pattern))
                {
                    res.Add(item);
                }
            }

            return res;
        }
    }
}
