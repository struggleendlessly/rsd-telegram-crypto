namespace api_alchemy.Eth.ResponseDTO
{
    public class getBalance
    {
        public string jsonrpc { get; set; } = string.Empty;
        public int id { get; set; }
        public string result { get; set; } = string.Empty;
    }
}
