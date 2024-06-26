namespace Shared.DB
{
    public class TokenInfo
    {
        public int Id { get; set; }

        public string HashContractTransaction { get; set; } = "";
        public string AddressToken { get; set; } = "";
        public string NameToken { get; set; } = "";
        public string AddressOwnersWallet { get; set; } = "";

        public string UrlToken { get; set; } = "";
        public string UrlOwnersWallet { get; set; } = "";
        public string UrlChart { get; set; } = "";
        public int TellMessageIdIsValid { get; set; } = 0;
        public int TellMessageIdBotVerified { get; set; } = 0;
        public int TellMessageIdNotVerified { get; set; } = 0;

        public bool IsValid { get; set; } = false;
        public bool IsProcessed1 { get; set; } = false;
        public string ErrorType { get; set; } = "";
        public bool IsProcessed2 { get; set; } = false;

        public int BlockNumber { get; set; }

        public DateTime TimeAdded { get; set; }
        public DateTime TimeUpdated { get; set; }

        public string symbol { get; set; } = "";
        public string divisor { get; set; } = "";
        public string tokenType { get; set; } = "";
        public string totalSupply { get; set; } = "";
        public string blueCheckmark { get; set; } = "";
        public string description { get; set; } = "";
        public string website { get; set; } = "";
        public string email { get; set; } = "";
        public string blog { get; set; } = "";
        public string reddit { get; set; } = "";
        public string slack { get; set; } = "";
        public string facebook { get; set; } = "";
        public string twitter { get; set; } = "";
        public string bitcointalk { get; set; } = "";
        public string github { get; set; } = "";
        public string telegram { get; set; } = "";
        public string wechat { get; set; } = "";
        public string linkedin { get; set; } = "";
        public string discord { get; set; } = "";
        public string whitepaper { get; set; } = "";
        public string tokenPriceUSD { get; set; } = "";

        //public List<TokenInfoUrl> TokenInfoUrls { get; } = new();
    }
}