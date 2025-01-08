using Microsoft.EntityFrameworkCore;

namespace dbMigration.models
{
    [Index(nameof(slotNumberStartInt), IsUnique = false)]
    [Index(nameof(slotNumberEndInt), IsUnique = false)]

    public class swapsTokens
    {
        public int Id { get; set; }
        public UInt64 slotNumberStartInt { get; set; }
        public UInt64 slotNumberEndInt { get; set; }

        public string addressToken { get; set; } = string.Empty;
        public string txsHash { get; set; } = string.Empty;

        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;

        public string solIn { get; set; } = string.Empty;
        public string solOut { get; set; } = string.Empty;
        public string tokenIn { get; set; } = string.Empty;
        public string tokenOut { get; set; } = string.Empty;

        public double priceTokenInSol { get; set; }
        public double priceSol_USD { get; set; }

        public bool isBuyToken { get; set; }
        public bool isBuySol { get; set; }

        public static swapsTokens Default(UInt64 block)
        {
            var res = new swapsTokens();
            res.slotNumberEndInt = block;

            return res;
        }
        public static swapsTokens Default()
        {
            var res = new swapsTokens();

            return res;
        }

    }
}
