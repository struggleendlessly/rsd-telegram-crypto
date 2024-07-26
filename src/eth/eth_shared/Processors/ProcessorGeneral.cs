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
            List<getTransactionReceiptDTO.Result> txnReceiptsFiltered)
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

                var txMetadata = txnReceiptsFiltered.Where(x => x.transactionHash == t.hash).FirstOrDefault();


                if (!isExistInDB &&
                    txMetadata is not null &&
                    txMetadata.logs.Count() > 1)
                {
                    t.contractAddress = txMetadata.contractAddress;

                    var logsJson = JsonSerializer.Serialize(txMetadata.logs);
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
