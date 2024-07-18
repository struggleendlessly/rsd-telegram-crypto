namespace Shared.DB
{
    public class GyosaRawData : IGyosa
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public decimal MarketCap { get; set; }
        public int TotalBribeAttempts { get; set; }
        public int SuccessfulBribes { get; set; }
        public double TotalBribeAttemptsETH { get; set; }
        public double SuccessfulBribesETH { get; set; }
        public double Controling { get; set; }
        public string CA { get; set; } = "";
        public string TimePosted { get; set; } = "";
    }
}
