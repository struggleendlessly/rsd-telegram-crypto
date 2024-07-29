using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using eth_shared.Map;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace eth_shared
{
    public class GetTransactions
    {
        private readonly dbContext dbContext;
        public GetTransactions(dbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<EthTrainData>> Start(
            List<Transaction> tokens,
            List<getTransactionReceiptDTO.Result> txnReceiptsFiltered,
            List<getTokenMetadataDTO> tokenMetadataFiltered,
            List<getTotalSupplyDTO> totalSupplyDTOFiltered)
        {
            var unfiltered = Get(tokens);
            var validated = await Validate(unfiltered);
            var res = Filter(validated, txnReceiptsFiltered, tokenMetadataFiltered, totalSupplyDTOFiltered);

            return res;
        }

        public async Task<int> End(List<EthTrainData> ethTrainDatas)
        {
            var res = 0;

            res = await SaveToDB(ethTrainDatas);

            return res;
        }

        private List<EthTrainData> Get(List<Transaction> tokens)
        {
            List<EthTrainData> res = new();

            foreach (var item in tokens)
            {
                var t = item.Map();

                if (t is null ||
                    t.input is null)
                {
                    continue;
                }

                if (t.input.StartsWith("0x6080") ||
                    t.input.StartsWith("0x6040")
                    )
                {
                    t.isCustomInputStart = false;
                }
                else
                {
                    t.isCustomInputStart = true;
                }

                t.blockNumberInt = Convert.ToInt32(t.blockNumber, 16);

                res.Add(t);

            }

            return res;
        }

        private async Task<List<EthTrainData>> Validate(List<EthTrainData> collection)
        {
            List<EthTrainData> res = new();

            foreach (var item in collection)
            {
                var isExistInDB = await dbContext.EthTrainData.AnyAsync(x => x.hash == item.hash);

                if (!isExistInDB)
                {
                    res.Add(item);
                }
            }

            return res;
        }

        private List<EthTrainData> Filter(
            List<EthTrainData> collection,
            List<getTransactionReceiptDTO.Result> txnReceiptsFiltered,
            List<getTokenMetadataDTO> tokenMetadataFiltered,
            List<getTotalSupplyDTO> totalSupplyDTOFiltered
            )
        {
            List<EthTrainData> res = new();

            foreach (var item in collection)
            {
                var txReceipt = txnReceiptsFiltered.Where(x => x.transactionHash == item.hash).FirstOrDefault();

                getTotalSupplyDTO? totalSupply = null;
                getTokenMetadataDTO? tokenMetadata = null;

                if (txReceipt != null)
                {
                    totalSupply = totalSupplyDTOFiltered.Where(x => x.id == txReceipt.txnNumberForMetadata).FirstOrDefault();
                    tokenMetadata = tokenMetadataFiltered.Where(x => x.id == txReceipt.txnNumberForMetadata).FirstOrDefault();
                }

                if (txReceipt is not null &&
                    totalSupply is not null &&
                    tokenMetadata is not null
                    )
                {
                    item.logo = tokenMetadata.result.logo;
                    item.decimals = (int)tokenMetadata.result.decimals;
                    item.name = tokenMetadata.result.name;
                    item.symbol = tokenMetadata.result.symbol;
                    item.totalSupply = totalSupply.result;
                    item.contractAddress = txReceipt.contractAddress;

                    var logsJson = JsonSerializer.Serialize(txReceipt.logs);
                    item.logs = logsJson;

                    res.Add(item);
                }
            }

            return res;
        }

        private async Task<int> SaveToDB(List<EthTrainData> ethTrainDatas)
        {
            var res = 0;

            dbContext.EthTrainData.AddRange(ethTrainDatas);
            res = await dbContext.SaveChangesAsync();

            return res;
        }
    }
}
