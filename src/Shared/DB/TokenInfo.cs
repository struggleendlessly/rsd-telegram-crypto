namespace Shared.DB
{
    public class TokenInfo
    {
        public int Id { get; set; }

        public string HashContractTransaction { get; set; } = "";
        public string AddressToken { get; set; } = "";
        public string AddressOwnersWallet { get; set; } = "";

        public string UrlToken { get; set; } = "";
        public string UrlOwnersWallet { get; set; } = "";
        public string UrlChart { get; set; } = "";

        public bool IsValid { get; set; } = false;
        public bool IsProcessed1 { get; set; } = false;
        public string ErrorType { get; set; } = "";
        public bool IsProcessed2 { get; set; } = false;

        public int BlockNumber { get; set; }

        public DateTime TimeAdded { get; set; }
        public DateTime TimeUpdated { get; set; }

        //public List<TokenInfoUrl> TokenInfoUrls { get; } = new();
    }
}