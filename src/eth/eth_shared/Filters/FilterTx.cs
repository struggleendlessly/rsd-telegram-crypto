using api_alchemy.Eth.ResponseDTO;

using System.Collections.Concurrent;

namespace eth_shared.Filters
{
    public class FilterTx
    {
        // 18160ddd - totalSupply()
        public static List<Transaction> FilterTokens_ToIsNull_TotalSupply(
            List<getBlockByNumberDTO> blocksFiltered)
        {
            List<Transaction> res = new();
            List<Transaction> transactions = new();

            foreach (var item in blocksFiltered)
            {
                transactions.AddRange(item.result.transactions);
            }

            foreach (var item in transactions)
            {
                if (string.IsNullOrEmpty(item.to) && 
                    item.input.Contains("18160ddd"))
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public static List<getTransactionReceiptDTO.Result> FilterTxnReceipts_LogsCount(
              ConcurrentBag<getTransactionReceiptDTO.Result> txnReceiptsUnfiltered)
        {
            List<getTransactionReceiptDTO.Result> res = new();

            foreach (var item in txnReceiptsUnfiltered)
            {
                if (item.logs.Count() > 1)
                {
                    res.Add(item);
                }
            }

            return res;
        }
    }
}
