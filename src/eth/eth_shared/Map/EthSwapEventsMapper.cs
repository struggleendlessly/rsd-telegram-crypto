using Data.Models;

using eth_shared.Extensions;

using Nethereum.ABI.FunctionEncoding;
using Nethereum.Util;

namespace eth_shared.Map
{
    public static class Mapper
    {
        static string EthAddress = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
        public static EthSwapEvents Map(
            this List<ParameterOutput> collection,
            EthTrainData ethTrainData,
            string decimalCeparator)
        {
            EthSwapEvents res = new();

            var sender = collection.Where(x => x.Parameter.Name.Equals("sender", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var to = collection.Where(x => x.Parameter.Name.Equals("to", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount0in = collection.Where(x => x.Parameter.Name.Equals("amount0in", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount1in = collection.Where(x => x.Parameter.Name.Equals("amount1in", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount0out = collection.Where(x => x.Parameter.Name.Equals("amount0out", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount1out = collection.Where(x => x.Parameter.Name.Equals("amount1out", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();

            List<string> listToOrder = [EthAddress, ethTrainData.contractAddress];
            listToOrder.Sort();

            BigDecimal EthIn = 0.0;
            BigDecimal EthOut = 0.0;
            BigDecimal TokenIn = 0.0;
            BigDecimal TokenOut = 0.0;

            if (listToOrder[0].Equals(EthAddress))
            {
                EthIn = BigDecimal.Parse(amount0in.FormatTo18(decimalCeparator));
                EthOut = BigDecimal.Parse(amount0out.FormatTo18(decimalCeparator));
                TokenIn = BigDecimal.Parse(amount1in.InsertComma(ethTrainData.decimals, decimalCeparator));
                TokenOut = BigDecimal.Parse(amount1out.InsertComma(ethTrainData.decimals, decimalCeparator));
            }
            else
            {
                EthIn = BigDecimal.Parse(amount1in.FormatTo18(decimalCeparator));
                EthOut = BigDecimal.Parse(amount1out.FormatTo18(decimalCeparator));
                TokenIn = BigDecimal.Parse(amount0in.InsertComma(ethTrainData.decimals, decimalCeparator));
                TokenOut = BigDecimal.Parse(amount0out.InsertComma(ethTrainData.decimals, decimalCeparator));
            }

            res.from = sender;
            res.to = to;
            res.EthIn = EthIn.ToString();
            res.EthOut = EthOut.ToString();
            res.TokenIn = TokenIn.ToString();
            res.TokenOut = TokenOut.ToString();

            return res;
        }

        public static EthSwapEventsETHUSD Map(
            this List<ParameterOutput> collection,
            string contractAddress,
            int decimals,
            string decimalCeparator)
        {
            EthSwapEventsETHUSD res = new();

            var sender = collection.Where(x => x.Parameter.Name.Equals("sender", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var to = collection.Where(x => x.Parameter.Name.Equals("to", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount0in = collection.Where(x => x.Parameter.Name.Equals("amount0in", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount1in = collection.Where(x => x.Parameter.Name.Equals("amount1in", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount0out = collection.Where(x => x.Parameter.Name.Equals("amount0out", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount1out = collection.Where(x => x.Parameter.Name.Equals("amount1out", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();

            List<string> listToOrder = [EthAddress, contractAddress];
            listToOrder.Sort();

            BigDecimal EthIn = 0.0;
            BigDecimal EthOut = 0.0;
            BigDecimal TokenIn = 0.0;
            BigDecimal TokenOut = 0.0;

            if (listToOrder[0].Equals(EthAddress))
            {
                EthIn = BigDecimal.Parse(amount0in.FormatTo18(decimalCeparator));
                EthOut = BigDecimal.Parse(amount0out.FormatTo18(decimalCeparator));
                TokenIn = BigDecimal.Parse(amount1in.InsertComma(decimals, decimalCeparator));
                TokenOut = BigDecimal.Parse(amount1out.InsertComma(decimals, decimalCeparator));
            }
            else
            {
                EthIn = BigDecimal.Parse(amount1in.FormatTo18(decimalCeparator));
                EthOut = BigDecimal.Parse(amount1out.FormatTo18(decimalCeparator));
                TokenIn = BigDecimal.Parse(amount0in.InsertComma(decimals, decimalCeparator));
                TokenOut = BigDecimal.Parse(amount0out.InsertComma(decimals, decimalCeparator));
            }

            res.from = sender;
            res.to = to;
            res.EthIn = EthIn.ToString();
            res.EthOut = EthOut.ToString();
            res.TokenIn = TokenIn.ToString();
            res.TokenOut = TokenOut.ToString();

            return res;
        }
    }
}
