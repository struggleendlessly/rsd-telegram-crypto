using dbMigration.models;

using Microsoft.EntityFrameworkCore;

namespace dbMigration
{

    public class ethDB(DbContextOptions<ethDB> options) : DbContext(options)
    {
        public required DbSet<EthTokenInfo> EthTokenInfoEntities { get; set; }
        public required DbSet<EthBlocksEntity> EthBlocksEntities { get; set; }
        public required DbSet<EthSwapsETH_USD> EthSwapsETH_USDEntities { get; set; }
        public required DbSet<EthSwapsETH_Token> EthSwapsETH_TokenEntities { get; set; }
    }
}