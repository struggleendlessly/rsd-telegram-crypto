namespace api_alchemy.Eth.ResponseDTO
{
    public class errorResponse
    {
        public Error error { get; set; } = new Error();
        public class Error
        {
            public string Code { get; set; } = "";
        }
    }
}
