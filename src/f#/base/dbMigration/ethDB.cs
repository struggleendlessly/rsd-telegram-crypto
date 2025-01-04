using ethCommonDB;
using ethCommonDB.models;

using Microsoft.EntityFrameworkCore;

namespace dbMigration
{
    public class ethDB(DbContextOptions<ethDB> options) : DbContext(options), IEthDB
    {
        public required DbSet<TokenInfo> EthTokenInfoEntities { get; set; }
        public required DbSet<BlocksEntity> EthBlocksEntities { get; set; }
        public required DbSet<SwapsETH_USD> EthSwapsETH_USDEntities { get; set; }
        public required DbSet<SwapsETH_Token> EthSwapsETH_TokenEntities { get; set; }


    }
}