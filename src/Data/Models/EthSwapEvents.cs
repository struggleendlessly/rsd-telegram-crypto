namespace Data.Models
{
    public class EthSwapEvents
    {
        public int Id { get; set; }

        public string pairAddress { get; set; } = string.Empty;
        public string txsHash { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
        public string EthIn { get; set; } = string.Empty;
        public string EthOut { get; set; } = string.Empty;
        public string TokenIn { get; set; } = string.Empty;
        public string TokenOut { get; set; } = string.Empty;
        public double priceEth { get; set; }
        public bool isBuy { get; set; }
        public int blockNumberInt { get; set; }

        public EthTrainData? EthTrainData { get; set; }
    }
}
