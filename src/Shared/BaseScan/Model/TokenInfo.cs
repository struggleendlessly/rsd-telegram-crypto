namespace Shared.BaseScan.Model
{
    public class TokenInfo
    {
        public string status { get; set; }
        public string message { get; set; }
        public Result[] result { get; set; }

        public class Result
        {
            public string contractAddress { get; set; }
            public string tokenName { get; set; }
            public string symbol { get; set; }
            public string divisor { get; set; }
            public string tokenType { get; set; }
            public string totalSupply { get; set; }
            public string blueCheckmark { get; set; }
            public string description { get; set; }
            public string website { get; set; }
            public string email { get; set; }
            public string blog { get; set; }
            public string reddit { get; set; }
            public string slack { get; set; }
            public string facebook { get; set; }
            public string twitter { get; set; }
            public string bitcointalk { get; set; }
            public string github { get; set; }
            public string telegram { get; set; }
            public string wechat { get; set; }
            public string linkedin { get; set; }
            public string discord { get; set; }
            public string whitepaper { get; set; }
            public string tokenPriceUSD { get; set; }
        }
    }
}
