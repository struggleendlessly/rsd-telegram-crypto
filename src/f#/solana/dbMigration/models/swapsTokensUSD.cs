using Microsoft.EntityFrameworkCore;

namespace dbMigration.models
{
    [Index(nameof(slotNumberInt), IsUnique = false)]

    public class swapsTokensUSD
    {
        public int Id { get; set; }
        public UInt64 slotNumberInt { get; set; }

        public string addressToken { get; set; } = string.Empty;
        public string txsHash { get; set; } = string.Empty;

        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;

        public double priceSolInUsd { get; set; }

        public bool isBuyDai { get; set; }
        public bool isBuySol { get; set; }

        public static swapsTokensUSD Default()
        {
            var res = new swapsTokensUSD();
            res.priceSolInUsd = 0.0;

            return res;
        }
    }
}
