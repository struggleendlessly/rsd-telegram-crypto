namespace api_alchemy.Eth.ResponseDTO
{
    public class responseBase
    {
        public string jsonrpc { get; set; } = string.Empty;
        public int id { get; set; }
        public Error error { get; set; } = new Error();
        public string result { get; set; } = string.Empty;
    }

    public class Error
    {
        public int code { get; set; }
        public string message { get; set; } = string.Empty;
    }
}
