namespace api_alchemy.Eth.ResponseDTO
{
    public class getSwapDTO
    {
        public string jsonrpc { get; set; } = string.Empty;
        public int id { get; set; }
        public Result[] result { get; set; } = [];


        public class Result
        {
            public string address { get; set; } = string.Empty;
            public string blockHash { get; set; } = string.Empty;
            public string blockNumber { get; set; } = string.Empty;
            public string data { get; set; } = string.Empty;
            public string logIndex { get; set; } = string.Empty;
            public bool removed { get; set; }
            public string[] topics { get; set; } = [];
            public string transactionHash { get; set; } = string.Empty;
            public string transactionIndex { get; set; } = string.Empty;
        }
    }
}
