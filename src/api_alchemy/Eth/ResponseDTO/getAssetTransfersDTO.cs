namespace api_alchemy.Eth.ResponseDTO
{
    public class getAssetTransfersDTO
    {
        public string jsonrpc { get; set; } = default!;
        public int id { get; set; } = default!;
        public Result result { get; set; } = default!;


        public class Result
        {
            public Transfer[] transfers { get; set; } = default!;
            public string pageKey { get; set; } = default!;
        }

        public class Transfer
        {
            public string blockNum { get; set; } = default!;
            public string uniqueId { get; set; } = default!;
            public string hash { get; set; } = default!;
            public string from { get; set; } = default!;
            public string to { get; set; } = default!;
            public float value { get; set; } = default!;
            public object erc721TokenId { get; set; } = default!;
            public object erc1155Metadata { get; set; } = default!;
            public object tokenId { get; set; } = default!;
            public string asset { get; set; } = default!;
            public string category { get; set; } = default!;
            public RawContract rawContract { get; set; } = default!;
            public Metadata metadata { get; set; } = default!;
        }

        public class RawContract
        {
            public string value { get; set; } = default!;
            public object address { get; set; } = default!;
            public string _decimal { get; set; } = default!;
        }

        public class Metadata
        {
            public DateTime blockTimestamp { get; set; } = default!;
        }
    }
}
