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
        public string price { get; set; } = string.Empty;
        public int blockNumberInt { get; set; }

        public virtual EthTrainData EthTrainData { get; set; }
    }
}
