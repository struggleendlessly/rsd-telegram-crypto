namespace Data.Models
{
    public class EthTrxOthers
    {
        public int Id { get; set; }

        // Custom fields
        public int blockNumberInt { get; set; } = 0;

        // Transaction
        public string blockHash { get; set; } = string.Empty;
        public string blockNumber { get; set; } = string.Empty;
        public string? hash { get; set; } = null;
        public string? yParity { get; set; } = null;
        //public Acce?sslist[] accessList { get; set; } = null;
        public string? transactionIndex { get; set; } = null;
        public string? type { get; set; } = null;
        public string? nonce { get; set; } = null;
        public string? input { get; set; } = null;
        public string? r { get; set; } = null;
        public string? s { get; set; } = null;
        public string? chainId { get; set; } = null;
        public string? v { get; set; } = null;
        public string? gas { get; set; } = null;
        public string? maxPriorityFeePerGas { get; set; } = null;
        public string? from { get; set; } = null;
        public string? to { get; set; } = null;
        public string? maxFeePerGas { get; set; } = null;
        public string? value { get; set; } = null;
        public string? gasPrice { get; set; } = null;
        public string? maxFeePerBlobGas { get; set; } = null;
        //public string[] blobVersionedHashes { get; set; } = null;

        //
    }
}
