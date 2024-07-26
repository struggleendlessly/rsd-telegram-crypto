namespace api_alchemy.Eth.ResponseDTO
{
    public class getTransactionReceiptDTO
    {
        public string jsonrpc { get; set; } = default!;
        public int id { get; set; } = default!;
        public Result result { get; set; } = default!;

        public class Result
        {
            public int txnNumberForMetadata { get; set; } = 0;
            public string transactionHash { get; set; } = default!;
            public string blockHash { get; set; } = default!;
            public string blockNumber { get; set; } = default!;
            public string logsBloom { get; set; } = default!;
            public string gasUsed { get; set; } = default!;
            public string contractAddress { get; set; } = default!;
            public string cumulativeGasUsed { get; set; } = default!;
            public string transactionIndex { get; set; } = default!;
            public string from { get; set; } = default!;
            public object to { get; set; } = default!;
            public string type { get; set; } = default!;
            public string effectiveGasPrice { get; set; } = default!;
            public Log[] logs { get; set; } = default!;
            public string status { get; set; } = default!;
        }

        public class Log
        {
            public string blockHash { get; set; } = default!;
            public string address { get; set; } = default!;
            public string logIndex { get; set; } = default!;
            public string data { get; set; } = default!;
            public bool removed { get; set; } = default!;
            public string[] topics { get; set; } = default!;
            public string blockNumber { get; set; } = default!;
            public string transactionIndex { get; set; } = default!;
            public string transactionHash { get; set; } = default!;
        }
    }
}
