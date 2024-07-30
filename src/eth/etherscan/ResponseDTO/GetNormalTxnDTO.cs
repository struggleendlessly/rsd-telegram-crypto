namespace etherscan.ResponseDTO
{
    public class GetNormalTxnDTO
    {
        public string? ownerAddresses { get; set; } = default;
        public string status { get; set; } = default!;
        public string message { get; set; } = default!;
        public Result[]? result { get; set; } = default;

        public class Result
        {
            public string? blockNumber { get; set; } = default;
            public string? blockHash { get; set; } = default;
            public string? timeStamp { get; set; } = default;
            public string? hash { get; set; } = default;
            public string? nonce { get; set; } = default;
            public string? transactionIndex { get; set; } = default;
            public string? from { get; set; } = default;
            public string? to { get; set; } = default;
            public string? value { get; set; } = default;
            public string? gas { get; set; } = default;
            public string? gasPrice { get; set; } = default;
            public string? input { get; set; } = default;
            public string? methodId { get; set; } = default;
            public string? functionName { get; set; } = default;
            public string? contractAddress { get; set; } = default;
            public string? cumulativeGasUsed { get; set; } = default;
            public string? txreceipt_status { get; set; } = default;
            public string? gasUsed { get; set; } = default;
            public string? confirmations { get; set; } = default;
            public string? isError { get; set; } = default;
        }
    }
}
