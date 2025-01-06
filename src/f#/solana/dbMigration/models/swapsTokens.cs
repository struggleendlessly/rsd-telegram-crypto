using Microsoft.EntityFrameworkCore;

namespace dbMigration.models
{
    public class swapsTokens
    {
        public int Id { get; set; }
        public UInt64 blockNumberStartInt { get; set; }
        public UInt64 blockNumberEndInt { get; set; }

        public string pairAddress { get; set; } = string.Empty;
        public string txsHash { get; set; } = string.Empty;

        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;

        public string SolIn { get; set; } = string.Empty;
        public string SolOut { get; set; } = string.Empty;
        public string TokenIn { get; set; } = string.Empty;
        public string TokenOut { get; set; } = string.Empty;

        public double priceTokenInSol { get; set; }
        public double priceSol_USD { get; set; }

        public bool isBuyToken { get; set; }
        public bool isBuySol { get; set; }

        public static swapsTokens Default(UInt64 block)
        {
            var res = new swapsTokens();
            res.blockNumberEndInt = block;

            return res;
        }

    }
}
