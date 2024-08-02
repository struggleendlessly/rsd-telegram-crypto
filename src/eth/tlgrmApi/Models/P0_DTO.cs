namespace tlgrmApi.Models
{
    public class P0_DTO
    {
        public int tlgrmMsgId { get; set; } = 0;
        public string name { get; set; } = string.Empty;
        public string symbol { get; set; } = string.Empty;
        public string contractAddress { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string totalSupply { get; set; } = string.Empty;
        public string walletAge { get; set; } = string.Empty;
        public string balanceOnCreating { get; set; } = string.Empty;
        public string threadId { get; set; } = string.Empty;
        public string ABI { get; set; } = string.Empty;
        public string messageText { get; set; } = string.Empty;
    }
}
