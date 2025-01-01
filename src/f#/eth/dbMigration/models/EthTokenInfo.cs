namespace dbMigration.models
{
    public class EthTokenInfo
    {
        public int Id { get; set; }

        // Block
        public string Address { get; set; } = string.Empty;
        public string NameLong { get; set; } = string.Empty;
        public string NameShort { get; set; } = string.Empty;
        public string Decimals { get; set; } = string.Empty;
    }
}
