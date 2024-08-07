using Data.Models;

using Nethereum.ABI.FunctionEncoding;

using System.Numerics;

namespace eth_shared.Map
{
    public static class EthSwapEventsMapper
    {
        public static EthSwapEvents Map(this List<ParameterOutput> collection)
        {
            EthSwapEvents res = new();

            var sender = collection.Where(x => x.Parameter.Name.Equals("sender", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var to = collection.Where(x => x.Parameter.Name.Equals("to", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount0in = collection.Where(x => x.Parameter.Name.Equals("amount0in", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount1in = collection.Where(x => x.Parameter.Name.Equals("amount1in", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount0out = collection.Where(x => x.Parameter.Name.Equals("amount0out", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();
            var amount1out = collection.Where(x => x.Parameter.Name.Equals("amount1out", StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault().Result.ToString();

            res.from = sender;
            res.to = to;
            res.amount0in = BigInteger.Parse(amount0in).ToString();
            res.amount1in = BigInteger.Parse(amount1in).ToString();
            res.amount0out = BigInteger.Parse(amount0out).ToString();
            res.amount1out = BigInteger.Parse(amount1out).ToString();

            return res;
        }
    }
}
