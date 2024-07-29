using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Nethereum.Hex.HexTypes;

using System.Numerics;

namespace eth_shared
{
    public class GetTotalSupply
    {
        private readonly EthApi apiAlchemy;
        public GetTotalSupply(EthApi apiAlchemy)
        {
            this.apiAlchemy = apiAlchemy;
        }

        public async Task<List<getTotalSupplyDTO>> Start(
            List<getTransactionReceiptDTO.Result> txnReceiptsFiltered,
            List<getTokenMetadataDTO> ethTrainDatas
            )
        {
            Thread.Sleep(1000);

            var unfiltered = await Get(txnReceiptsFiltered);
            var validated = Validate(unfiltered);
            var res = Filter(ethTrainDatas, validated);

            return res;
        }

        private async Task<List<getTotalSupplyDTO>> Get(List<getTransactionReceiptDTO.Result> txnReceiptsFiltered)
        {
            List<getTotalSupplyDTO> res = new();

            var diff = txnReceiptsFiltered.Count();
            var items = txnReceiptsFiltered;

            Func<List<getTransactionReceiptDTO.Result>, int, Task<List<getTotalSupplyDTO>>> apiMethod = apiAlchemy.getTotalSupplyBatch;

            res = await apiAlchemy.executeBatchCall(items, apiMethod, diff);

            return res;
        }

        public List<getTotalSupplyDTO> Validate(List<getTotalSupplyDTO> collection)
        {
            List<getTotalSupplyDTO> res = new();

            foreach (var item in collection)
            {
                if (item is not null)
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public static List<getTotalSupplyDTO> Filter(
            List<getTokenMetadataDTO> tokenMetadataDTOs,
            List<getTotalSupplyDTO> totalSupplies)
        {
            List<getTotalSupplyDTO> res = new();

            foreach (var item in totalSupplies)
            {
                var tokenMetadata = tokenMetadataDTOs.Find(x => x.id == item.id);

                if (tokenMetadata is null ||
                    tokenMetadata.result is null ||
                    tokenMetadata.result.decimals is null)
                {
                    continue;
                }

                var decimals = (int)tokenMetadata.result.decimals;

                BigInteger totalSupply = 0;

                totalSupply = new HexBigInteger(item.result).Value;

                for (int i = 0; i < decimals; i++)
                {
                    totalSupply /= 10;
                }

                if (totalSupply >= 1_000_000)
                {
                    item.result = totalSupply.ToString();
                    res.Add(item);
                }
            }

            return res;
        }
    }
}
