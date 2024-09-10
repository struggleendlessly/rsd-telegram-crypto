using Data.Models;

namespace tlgrmApi.Models
{
    public class P0_DTO
    {
        public int EthTrainDataId { get; set; } = 0;
        public EthTrainData EthTrainData { get; set; } = new();
        public int tlgrmMsgId { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string symbol { get; set; } = string.Empty;
        public string contractAddress { get; set; } = string.Empty;
        public string pairAddress { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string totalSupply { get; set; } = string.Empty;
        public string walletAge { get; set; } = string.Empty;
        public string balanceOnCreating { get; set; } = string.Empty;
        public string threadId { get; set; } = string.Empty;
        public string ABI { get; set; } = string.Empty;
        public string messageText { get; set; } = string.Empty;
        public string ABIICon { get; set; } = string.Empty;
        public string walletIcon { get; set; } = string.Empty;
        public string balanceIcon { get; set; } = string.Empty;
        public string currency { get; set; } = "ETH";
        public string line_tokenName { get; set; } = string.Empty;
        public string line_tokenAddress { get; set; } = string.Empty;
        public string line_tokenSupply { get; set; } = string.Empty;
        public string line_WalletAgeAndBalance { get; set; } = string.Empty;
        public string line_WalletFromType { get; set; } = string.Empty;
        public string line_DextoolsAndDexcreener { get; set; } = string.Empty;
    }
}
