﻿using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    [Index(nameof(blockNumberInt), IsUnique = false)]
    public class EthSwapEventsETHUSD
    {
        public int Id { get; set; }

        public string pairAddress { get; set; } = string.Empty;
        public string txsHash { get; set; } = string.Empty;
        public string from { get; set; } = string.Empty;
        public string to { get; set; } = string.Empty;
        public string EthIn { get; set; } = string.Empty;
        public string EthOut { get; set; } = string.Empty;
        public string TokenIn { get; set; } = string.Empty;
        public string TokenOut { get; set; } = string.Empty;
        public double priceEthInUsd { get; set; }
        public bool isBuyDai { get; set; }
        public bool isBuyEth { get; set; }
        public int blockNumberInt { get; set; }
    }
}