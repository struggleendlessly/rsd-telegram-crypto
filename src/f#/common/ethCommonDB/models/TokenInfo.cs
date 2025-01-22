using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace ethCommonDB.models
{
    [Index(nameof(AddressToken), IsUnique = false)]
    [Index(nameof(AddressPair), IsUnique = false)]
    public class TokenInfo
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
        public string? TotalSupply { get; set; } = string.Empty;

        public static List<TokenInfo> Default()
        {
            var res = new List<TokenInfo>();

            return res;
        }
    }
}
