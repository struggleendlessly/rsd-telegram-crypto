using api_alchemy.Eth.ResponseDTO;

using Data.Models;

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

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

        // name != "test" or anything with symbols like  .  / , > < ;   
        public static List<getTokenMetadataDTO> FilterMetadata_Names(
              ConcurrentBag<getTokenMetadataDTO> metadata)
        {
            List<getTokenMetadataDTO> res = new();

            foreach (var item in metadata)
            {
                var name = item.result.name;
                var decimals = item.result.decimals;
                string pattern = @"\p{IsCyrillic}";

                if (decimals is not null &&
                name is not null &&
                Regex.Matches(name, pattern).Count == 0 &&
                !name.Contains('.') &&
                !name.Contains(',') &&
                !name.Contains('<') &&
                !name.Contains('>') &&
                !name.Contains('$') &&
                !name.Contains(';') &&
                !name.Contains("test", StringComparison.OrdinalIgnoreCase) &&
                !name.Contains('/')
                )
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public static List<EthTrainData> FilterTotalSupply(
              List<EthTrainData> ethTrainDatas)
        {
            List<EthTrainData> res = new();

            foreach (var item in ethTrainDatas)
            {
                if (item.totalSupply >= 1_000_000)
                {
                    res.Add(item);
                }
            }

            return res;
        }
    }
}
