using Data.Models;

using Nethereum.Util;

namespace Shared.DTO
{
    public class EthSwapEventsDTO
    {
        public int Id { get; set; }

        public string pairAddress { get; set; } = string.Empty;
        public string txsHash { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
        public BigDecimal EthIn { get; set; }
        public BigDecimal EthOut { get; set; }
        public BigDecimal TokenIn { get; set; }
        public BigDecimal TokenOut { get; set; }
        public double priceEth { get; set; }
        public bool isBuy { get; set; }
        public int blockNumberInt { get; set; }
        public string tokenNotEth { get; set; } = string.Empty;

        public EthTrainData? EthTrainData { get; set; }
    }
}
