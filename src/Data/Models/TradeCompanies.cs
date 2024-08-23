namespace Data.Models
{
    public class TradeCompanies
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public ICollection<WalletNames> WalletNames { get; set; } = [];
    }
}
