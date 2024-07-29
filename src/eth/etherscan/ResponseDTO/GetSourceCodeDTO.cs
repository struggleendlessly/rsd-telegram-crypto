namespace etherscan.ResponseDTO
{
    public class GetSourceCodeDTO
    {
        public string contractAddress { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public List<Result> result { get; set; } = new();

        public class Result
        {
            public string SourceCode { get; set; } = string.Empty;
            public string ABI { get; set; } = string.Empty;
            public string ContractName { get; set; } = string.Empty;
            public string CompilerVersion { get; set; } = string.Empty;
            public string OptimizationUsed { get; set; } = string.Empty;
            public string Runs { get; set; } = string.Empty;
            public string ConstructorArguments { get; set; } = string.Empty;
            public string EVMVersion { get; set; } = string.Empty;
            public string Library { get; set; } = string.Empty;
            public string LicenseType { get; set; } = string.Empty;
            public string Proxy { get; set; } = string.Empty;
            public string Implementation { get; set; } = string.Empty;
            public string SwarmSource { get; set; } = string.Empty;
        }
    }
}
