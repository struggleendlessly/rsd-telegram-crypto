﻿using Microsoft.EntityFrameworkCore;

namespace ethCommonDB.models
{
    [Index(nameof(numberInt), IsUnique = true)]
    public class BlocksEntity
    {
        public int Id { get; set; }

        // Block
        public string numberHex { get; set; } = string.Empty;
        public int numberInt { get; set; } = 0;

        public string timestampUnix { get; set; } = string.Empty;
        public DateTime timestampNormal { get; set; } = new();

        public string baseFeePerGas { get; set; } = string.Empty;
        public string gasLimit { get; set; } = string.Empty;
        public string gasUsed { get; set; } = string.Empty;

        public string hash { get; set; } = string.Empty;

        public static BlocksEntity Default(int block)
        {
            var res = new BlocksEntity();
            res.numberInt = block;

            return res;
        }
    }
}
