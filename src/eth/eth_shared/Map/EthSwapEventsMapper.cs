using Data.Models;

using eth_shared.Extensions;

using Nethereum.ABI.FunctionEncoding;
using Nethereum.Util;

using Shared.DTO;

using static eth_shared.GetSwapEvents;

namespace eth_shared.Map
{
    public static class Mapper
    {
        static readonly string EthAddress = "0xC02aaA39b223FE8D0A0e5C4F27eAD9083C756Cc2";
        public static EthSwapEvents Map(
            this List<ParameterOutput> collection,
            EthTrainData ethTrainData,
            string decimalCeparator,
            Token0AndToken1 Token01)
        {
            EthSwapEvents res = new();
            var EthAddressMaybe = EthAddress;

            var sender = collection.Where(x => x.Parameter.Name.Equals("sender", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var to = collection.Where(x => x.Parameter.Name.Equals("to", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount0in = collection.Where(x => x.Parameter.Name.Equals("amount0in", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount1in = collection.Where(x => x.Parameter.Name.Equals("amount1in", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount0out = collection.Where(x => x.Parameter.Name.Equals("amount0out", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount1out = collection.Where(x => x.Parameter.Name.Equals("amount1out", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();

            List<string> listToOrder = [EthAddress, ethTrainData.contractAddress];

            if (Token01.token0.Contains(EthAddress, StringComparison.InvariantCultureIgnoreCase) ||
                Token01.token1.Contains(EthAddress, StringComparison.InvariantCultureIgnoreCase))
            {

            }
            else
            {
                if (Token01.token0.Contains(ethTrainData.contractAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    res.tokenNotEth = Token01.token1;
                    EthAddressMaybe = Token01.token1;
                }

                if (Token01.token1.Contains(ethTrainData.contractAddress, StringComparison.InvariantCultureIgnoreCase))
                {
                    res.tokenNotEth = Token01.token0;
                    EthAddressMaybe = Token01.token0;
                }

            }

            listToOrder = [Token01.token0, Token01.token1];

            BigDecimal EthIn = 0.0;
            BigDecimal EthOut = 0.0;
            BigDecimal TokenIn = 0.0;
            BigDecimal TokenOut = 0.0;

            if (listToOrder[0].Equals(EthAddressMaybe, StringComparison.CurrentCultureIgnoreCase))
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

        public static EthSwapEventsDTO Map(
            this EthSwapEvents val)
        {
            EthSwapEventsDTO res = new();

            res.pairAddress = val.pairAddress;
            res.txsHash = val.txsHash;
            res.from = val.from;
            res.to = val.to;
            res.EthIn = BigDecimal.Parse(val.EthIn);
            res.EthOut = BigDecimal.Parse(val.EthOut);
            res.TokenIn = BigDecimal.Parse(val.TokenIn);
            res.TokenOut = BigDecimal.Parse(val.TokenOut);
            res.priceEth = val.priceEth;
            res.isBuy = val.isBuy;
            res.blockNumberInt = val.blockNumberInt;
            res.tokenNotEth = val.tokenNotEth;
            res.Id = val.Id;
            res.EthTrainDataId = (int)val.EthTrainDataId;

            return res;
        }
    }
}
