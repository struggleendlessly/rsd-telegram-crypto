namespace api_alchemy.Eth.ResponseDTO
{
    public class getBlockByNumberDTO
    {
        public string jsonrpc { get; set; } = default!;
        public int id { get; set; } = default!;
        public Result result { get; set; } = default!;
    }

    public class Result
    {
        public string number { get; set; } = default!;
        public string hash { get; set; } = default!;
        public Transaction[] transactions { get; set; } = default!;
        public string logsBloom { get; set; } = default!;
        public string totalDifficulty { get; set; } = default!;
        public string receiptsRoot { get; set; } = default!;
        public string extraData { get; set; } = default!;
        public string withdrawalsRoot { get; set; } = default!;
        public string baseFeePerGas { get; set; } = default!;
        public string nonce { get; set; } = default!;
        public string miner { get; set; } = default!;
        public Withdrawal[] withdrawals { get; set; } = default!;
        public string excessBlobGas { get; set; } = default!;
        public string difficulty { get; set; } = default!;
        public string gasLimit { get; set; } = default!;
        public string gasUsed { get; set; } = default!;
        public object[] uncles { get; set; } = default!;
        public string parentBeaconBlockRoot { get; set; } = default!;
        public string size { get; set; } = default!;
        public string sha3Uncles { get; set; } = default!;
        public string transactionsRoot { get; set; } = default!;
        public string stateRoot { get; set; } = default!;
        public string mixHash { get; set; } = default!;
        public string parentHash { get; set; } = default!;
        public string blobGasUsed { get; set; } = default!;
        public string timestamp { get; set; } = default!;
    }

    public class Transaction
    {
        public string blockHash { get; set; } = default!;
        public string blockNumber { get; set; } = default!;
        public string hash { get; set; } = default!;
        public string yParity { get; set; } = default!;
        public Accesslist[] accessList { get; set; } = default!;
        public string transactionIndex { get; set; } = default!;
        public string type { get; set; } = default!;
        public string nonce { get; set; } = default!;
        public string input { get; set; } = default!;
        public string r { get; set; } = default!;
        public string s { get; set; } = default!;
        public string chainId { get; set; } = default!;
        public string v { get; set; } = default!;
        public string gas { get; set; } = default!;
        public string maxPriorityFeePerGas { get; set; } = default!;
        public string from { get; set; } = default!;
        public string to { get; set; } = default!;
        public string maxFeePerGas { get; set; } = default!;
        public string value { get; set; } = default!;
        public string gasPrice { get; set; } = default!;
        public string maxFeePerBlobGas { get; set; } = default!;
        public string[] blobVersionedHashes { get; set; } = default!;
    }

    public class Accesslist
    {
        public string address { get; set; } = default!;
        public string[] storageKeys { get; set; } = default!;
    }

    public class Withdrawal
    {
        public string amount { get; set; } = default!;
        public string address { get; set; } = default!;
        public string index { get; set; } = default!;
        public string validatorIndex { get; set; } = default!;
    }

}
