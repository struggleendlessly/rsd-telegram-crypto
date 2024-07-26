namespace api_alchemy.Eth.ResponseDTO
{
    public class getTokenMetadataDTO
    {
        public string jsonrpc { get; set; } = default!;
        public int id { get; set; } = default!;
        public Result result { get; set; } = new Result();

        public class Result
        {
            public int? decimals { get; set; } = null;
            public string? logo { get; set; } = null;
            public string? name { get; set; } = null;
            public string? symbol { get; set; } = null;
        }
    }
}
