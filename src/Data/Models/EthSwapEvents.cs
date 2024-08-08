namespace Data.Models
{
    public class EthSwapEvents
    {
        public int Id { get; set; }

        public string pairAddress { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
        public string amount0in { get; set; } = string.Empty;
        public string amount1in { get; set; } = string.Empty;
        public string amount0out { get; set; } = string.Empty;
        public string amount1out { get; set; } = string.Empty;
        public double priceEth { get; set; }
        public bool isBuy { get; set; }
        public int blockNumberInt { get; set; }

        public EthTrainData? EthTrainData { get; set; }
    }
}
