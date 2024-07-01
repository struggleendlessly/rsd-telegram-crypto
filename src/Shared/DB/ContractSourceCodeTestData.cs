namespace Shared.DB
{
    public class ContractSourceCodeTestData
    {
        public int Id { get; set; }
        public string didXXX { get; set; } = "";
        public bool isGood { get; set; }
        public string typeOfScam { get; set; } = "";

        public string AddressToken { get; set; } = "";
        public string SourceCode { get; set; } = "";
        public string ABI { get; set; } = "";
        public string ContractName { get; set; } = "";
        public string CompilerVersion { get; set; } = "";
        public string OptimizationUsed { get; set; } = "";
        public string Runs { get; set; } = "";
        public string ConstructorArguments { get; set; } = "";
        public string EVMVersion { get; set; } = "";
        public string Library { get; set; } = "";
        public string LicenseType { get; set; } = "";
        public string Proxy { get; set; } = "";
        public string Implementation { get; set; } = "";
        public string SwarmSource { get; set; } = "";
    }
}
