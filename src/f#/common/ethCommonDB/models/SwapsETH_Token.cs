﻿using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace ethCommonDB.models
{
    [Index(nameof(blockNumberStartInt), IsUnique = false)]
    [Index(nameof(blockNumberEndInt), IsUnique = false)]

    public class SwapsETH_Token
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

        [Column(TypeName = "decimal(38, 20)")]
        public decimal priceTokenInETH { get; set; }
        public double priceETH_USD { get; set; }

        public bool isBuyToken { get; set; }
        public bool isBuyEth { get; set; }

        public static SwapsETH_Token Default(int block)
        {
            var res = new SwapsETH_Token();
            res.blockNumberEndInt = block;

            return res;
        }
    }
}
