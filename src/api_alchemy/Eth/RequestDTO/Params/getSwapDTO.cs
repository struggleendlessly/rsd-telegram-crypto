namespace api_alchemy.Eth.RequestDTO.Params
{
    public class getSwapDTO
    {
        public string fromBlock { get; set; } = string.Empty;
        public string toBlock { get; set; } = string.Empty;
        public string[] address { get; set; } = [];
        public string[] topics { get; set; } = [];
    }
}
