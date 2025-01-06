using dbMigration.models;

using Microsoft.EntityFrameworkCore;

namespace dbMigration
{
    public class solDB(DbContextOptions<solDB> options) : DbContext(options)
    {
        public required DbSet<slots> slotsEntities { get; set; }
        public required DbSet<swapsTokens> swapsTokensEntities { get; set; }
        //public required DbSet<SwapsETH_USD> swapsETH_USDEntities { get; set; }
        //public required DbSet<SwapsETH_Token> swapsETH_TokenEntities { get; set; }

    }
}
