namespace api_alchemy.Eth.ResponseDTO
{
    public class getTotalSupplyDTO
    {
        public string jsonrpc { get; set; } = string.Empty;
        public int id { get; set; }
        public Error error { get; set; } = new Error();
        public string result { get; set; } = string.Empty;
    }
}