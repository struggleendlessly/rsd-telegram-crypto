namespace Shared.DB
{
    public interface IGyosa
    {
        string CA { get; set; }
        double Controling { get; set; }
        int Id { get; set; }
        decimal MarketCap { get; set; }
        int SuccessfulBribes { get; set; }
        double SuccessfulBribesETH { get; set; }
        string TimePosted { get; set; }
        string Title { get; set; }
        int TotalBribeAttempts { get; set; }
        double TotalBribeAttemptsETH { get; set; }
    }
}