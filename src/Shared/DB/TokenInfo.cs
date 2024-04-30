namespace Shared.DB
{
    public class TokenInfo
    {
        public int Id { get; set; }

        public string AddressToken { get; set; }
        public string AddressOwnersWallet { get; set; }
        public int TelegramMessageId { get; set; }
        public string TelegramMessage { get; set; }

        public string UrlToken { get; set; }
        public string UrlOwnersWallet { get; set; }
        public string UrlChart { get; set; }

        public bool IsProcessed1 { get; set; }
        public bool IsProcessed2 { get; set; }

        public DateTime TimeAdded { get; set; }
        public DateTime TimeUpdated { get; set; }

        //public List<TokenInfoUrl> TokenInfoUrls { get; } = new();
    }
}
