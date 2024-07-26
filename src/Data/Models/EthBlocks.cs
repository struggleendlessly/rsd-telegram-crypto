using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    [Index(nameof(numberInt), IsUnique = true)]
    public class EthBlocks
    {
        public int Id { get; set; }

        // Block
        public string number { get; set; } = string.Empty;
        public int numberInt { get; set; } = 0;
        public string baseFeePerGas { get; set; } = string.Empty;
        public string gasLimit { get; set; } = string.Empty;
        public string gasUsed { get; set; } = string.Empty;
        public string timestamp { get; set; } = string.Empty;

        //public string hash { get; set; } = string.Empty;
        //public string logsBloom { get; set; } = string.Empty;
        //public string totalDifficulty { get; set; } = string.Empty;
        //public string receiptsRoot { get; set; } = string.Empty;
        //public string extraData { get; set; } = string.Empty;
        //public string withdrawalsRoot { get; set; } = string.Empty;
        //public string nonce { get; set; } = string.Empty;
        //public string miner { get; set; } = string.Empty;
        //public string excessBlobGas { get; set; } = string.Empty;
        //public string difficulty { get; set; } = string.Empty;
        //public string parentBeaconBlockRoot { get; set; } = string.Empty;
        //public string size { get; set; } = string.Empty;
        //public string sha3Uncles { get; set; } = string.Empty;
        //public string transactionsRoot { get; set; } = string.Empty;
        //public string stateRoot { get; set; } = string.Empty;
        //public string mixHash { get; set; } = string.Empty;
        //public string parentHash { get; set; } = string.Empty;
        //public string blobGasUsed { get; set; } = string.Empty;

    }
}
