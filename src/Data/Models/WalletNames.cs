namespace Data.Models
{
    public class WalletNames
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";

        public TradeCompanies TradeCompanies { get; set; } = new();
    }
}
