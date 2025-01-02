using Microsoft.EntityFrameworkCore;

namespace dbMigration.models
{
    [Index(nameof(blockNumberStartInt), IsUnique = false)]
    [Index(nameof(blockNumberEndInt), IsUnique = false)]

    public class EthSwapsETH_Token
    {
        public int Id { get; set; }
        public int blockNumberStartInt { get; set;  }
        public int blockNumberEndInt { get; set;  }

        public string pairAddress { get; set; } = string.Empty;
        public string txsHash { get; set; } = string.Empty;

        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;

        public string EthIn { get; set; } = string.Empty;
        public string EthOut { get; set; } = string.Empty;
        public string TokenIn { get; set; } = string.Empty;
        public string TokenOut { get; set; } = string.Empty;

        public double priceTokenInETH { get; set; }
        public double priceETH_USD { get; set; }

        public bool isBuyToken { get; set; }
        public bool isBuyEth { get; set; }

        public static EthSwapsETH_Token Default()
        {
            var res = new EthSwapsETH_Token();
            res.blockNumberEndInt = staticValues.blockNumberInt;

            return res;
        }
    }
}
