using Microsoft.EntityFrameworkCore;

using System.ComponentModel;

namespace Data.Models
{
    [Index(nameof(hash), IsUnique = true)]
    [Index(nameof(blockNumberInt))]
    [Index(nameof(contractAddress), IsUnique = true)]
    public class EthTrainData
    {
        public int Id { get; set; }

        // Custom fields
        public bool isCustomInputStart { get; set; } = false;
        public DateTime walletCreated { get; set; }

        [DefaultValue(-1.0)]
        public double BalanceOnCreating { get; set; } = -1;
        public int blockNumberInt { get; set; } = 0;
        public string totalSupply { get; set; } = default!;
        public string tsExploits { get; set; } = string.Empty;
        public string tsFullResponse { get; set; } = string.Empty;
        public string pairAddress { get; set; } = string.Empty;
        public int pairBlockNumberInt { get; set; }
        public string pairAddressFunctionName { get; set; } = string.Empty;
        public bool isDead { get; set; } = false;
        public int isDeadInt { get; set; }
        public int DeadBlockNumber { get; set; } = 0;
        public string WalletSource1in { get; set; } = "";
        public int WalletSource1inCountRemLiq { get; set; }

        public ICollection<EthSwapEvents> EthSwapEvents { get; set; } = [];
        public ICollection<EthTokensVolume> EthTokensVolumes { get; set; } = [];

        // Telegram
        public long tlgrmNewTokens { get; set; } = 0;
        public long tlgrmLivePairs { get; set; } = 0;
        public long tlgrmVolume { get; set; } = 0;

        // TokenMetadata
        public int decimals { get; set; } = default!;
        public string? logo { get; set; } = null;
        public string name { get; set; } = default!;
        public string? symbol { get; set; } = string.Empty;

        // TransactionReceipt
        public string logs { get; set; } = string.Empty;
        public string contractAddress { get; set; } = string.Empty;

        // Transaction
        public string blockHash { get; set; } = string.Empty;
        public string blockNumber { get; set; } = string.Empty;
        public string hash { get; set; } = string.Empty;
        public string? yParity { get; set; } = null;
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

        //SourceCode
        public string? SourceCode { get; set; } = null;
        public string? ABI { get; set; } = null;
        public string? ContractName { get; set; } = null;
        public string? CompilerVersion { get; set; } = null;
        public string? OptimizationUsed { get; set; } = null;
        public string? Runs { get; set; } = null;
        public string? ConstructorArguments { get; set; } = null;
        public string? EVMVersion { get; set; } = null;
        public string? Library { get; set; } = null;
        public string? LicenseType { get; set; } = null;
        public string? Proxy { get; set; } = null;
        public string? Implementation { get; set; } = null;
        public string? SwarmSource { get; set; } = null;
    }
}
