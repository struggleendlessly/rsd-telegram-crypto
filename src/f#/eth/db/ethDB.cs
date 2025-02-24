using ethCommonDB;
using ethCommonDB.models;

using Microsoft.EntityFrameworkCore;

namespace dbMigration
{

    public class ethDB(DbContextOptions<ethDB> options) : DbContext(options), IEthDB
    {
        public required DbSet<TriggerHistory> triggerHistoriesEntities { get; set; }
        public required DbSet<TokenInfo> tokenInfoEntities { get; set; }
        public required DbSet<BlocksEntity> blocksEntities { get; set; }
        public required DbSet<SwapsETH_USD> swapsETH_USDEntities { get; set; }
        public required DbSet<SwapsETH_Token> swapsETH_TokenEntities { get; set; }
        public required DbSet<SwapsETH_Token_30mins> swapsETH_Token_30MinsEntities { get; set; }
    }
}