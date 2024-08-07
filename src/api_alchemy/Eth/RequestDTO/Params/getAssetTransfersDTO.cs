namespace api_alchemy.Eth.RequestDTO.Params
{
    public class getAssetTransfersDTO
    {
        public string fromBlock { get; set; } = string.Empty;
        public string toBlock { get; set; } = string.Empty;
        public string toAddress { get; set; } = string.Empty;
        public string[] category { get; set; } = [];
        public bool withMetadata { get; set; }
        public bool excludeZeroValue { get; set; }
        public string maxCount { get; set; } = string.Empty;
    }
}
