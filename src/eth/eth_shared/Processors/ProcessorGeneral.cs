using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using eth_shared.Map;

using Microsoft.EntityFrameworkCore;

using System.Text.Json;

namespace eth_shared.Processors
{
    public class ProcessorGeneral
    {
        private readonly dbContext dbContext;

        public ProcessorGeneral(
            dbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<List<EthTrainData>> ProcessTokens(
            List<Transaction> tokens,
            List<getTransactionReceiptDTO.Result> txnReceiptsFiltered, 
            List<getTokenMetadataDTO> tokenMetadataFiltered)
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

                var isExistInDB = await dbContext.EthTrainData.AnyAsync(x => x.hash == t.hash);

                var txReceipt = txnReceiptsFiltered.Where(x => x.transactionHash == t.hash).FirstOrDefault();

                getTokenMetadataDTO? tokenMetadata = null;

                if (txReceipt != null)
                {
                    tokenMetadata = tokenMetadataFiltered.Where(x => x.id == txReceipt.txnNumberForMetadata).FirstOrDefault();
                }

                if (!isExistInDB &&
                    txReceipt is not null &&
                    tokenMetadata is not null
                    )
                {
                    t.logo = tokenMetadata.result.logo;
                    t.decimals = (int)tokenMetadata.result.decimals;
                    t.name = tokenMetadata.result.name;
                    t.symbol = tokenMetadata.result.symbol;

                    t.contractAddress = txReceipt.contractAddress;

                    var logsJson = JsonSerializer.Serialize(txReceipt.logs);
                    t.logs = logsJson;

                    res.Add(t);
                }
            }

            return res;
        }

        //private List<EthTrxOthers> ProcessOthers()
        //{
        //    List<EthTrxOthers> res = new();

        //    foreach (var item in others)
        //    {
        //        var t = item.MapOthers();

        //        t.blockNumberInt = Convert.ToInt32(t.blockNumber, 16);

        //        var isExist = dbContext.EthTrxOther.Any(x => x.hash == t.hash);

        //        if (!isExist)
        //        {
        //            res.Add(t);
        //        }
        //    }

        //    return res;
        //}

        public async Task<List<EthBlocks>> ProcessBlocks(
            List<getBlockByNumberDTO> blocksFiltered)
        {
            List<EthBlocks> res = new();

            foreach (var item in blocksFiltered)
            {
                var t = item.Map();

                if (string.IsNullOrEmpty(t.number))
                {
                    continue;
                }

                t.numberInt = Convert.ToInt32(t.number, 16);

                var isExist = await dbContext.EthBlock.AnyAsync(x => x.numberInt == t.numberInt);

                if (!isExist)
                {
                    res.Add(t);
                }
            }

            return res;
        }

    }
}
