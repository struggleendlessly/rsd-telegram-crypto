namespace api_alchemy.Eth.ResponseDTO
{
    public class getTransactionReceiptDTO
    {
        public string jsonrpc { get; set; }
        public int id { get; set; }
        public Result result { get; set; }

        public class Result
        {
            public string transactionHash { get; set; }
            public string blockHash { get; set; }
            public string blockNumber { get; set; }
            public string logsBloom { get; set; }
            public string gasUsed { get; set; }
            public string contractAddress { get; set; }
            public string cumulativeGasUsed { get; set; }
            public string transactionIndex { get; set; }
            public string from { get; set; }
            public object to { get; set; }
            public string type { get; set; }
            public string effectiveGasPrice { get; set; }
            public Log[] logs { get; set; }
            public string status { get; set; }
        }

        public class Log
        {
            public string blockHash { get; set; }
            public string address { get; set; }
            public string logIndex { get; set; }
            public string data { get; set; }
            public bool removed { get; set; }
            public string[] topics { get; set; }
            public string blockNumber { get; set; }
            public string transactionIndex { get; set; }
            public string transactionHash { get; set; }
        }
    }
}
