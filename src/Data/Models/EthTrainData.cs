using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    [Index(nameof(hash), IsUnique = true)]
    public class EthTrainData
    {
        public int Id { get; set; }

        // Custom fields
        public bool isCustomInputStart { get; set; } = false;
        public int blockNumberInt { get; set; } = 0;
        // TransactionReceipt

        public string logs { get; set; } = string.Empty;
        public string contractAddress { get; set; } = string.Empty;

        // Transaction
        public string blockHash { get; set; } = string.Empty;
        public string blockNumber { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string? yParity { get; set; } = null;
        //public Acce?sslist[] accessList { get; set; }
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
        //public string[] blobVersionedHashes { get; set; }

        //
    }
}
