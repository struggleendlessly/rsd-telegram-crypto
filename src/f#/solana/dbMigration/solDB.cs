using Microsoft.EntityFrameworkCore;

namespace dbMigration
{
    public class solDB(DbContextOptions<solDB> options) : DbContext(options)
    {
        //public required DbSet<TokenInfo> tokenInfoEntities { get; set; }
        //public required DbSet<BlocksEntity> blocksEntities { get; set; }
        //public required DbSet<SwapsETH_USD> swapsETH_USDEntities { get; set; }
        //public required DbSet<SwapsETH_Token> swapsETH_TokenEntities { get; set; }

    }
}
