using Microsoft.EntityFrameworkCore;

namespace dbMigration.models
{
    [Index(nameof(AddressToken), IsUnique = false)]
    [Index(nameof(AddressPair), IsUnique = false)]
    public class EthTokenInfo
    {
        public int Id { get; set; }

        // Block
        public string AddressToken { get; set; } = string.Empty;
        public string AddressToken0 { get; set; } = string.Empty;
        public string AddressToken1 { get; set; } = string.Empty;
        public string AddressPair { get; set; } = string.Empty;
        public string? NameLong { get; set; } = string.Empty;
        public string? NameShort { get; set; } = string.Empty;
        public int Decimals { get; set; }
    }
}
