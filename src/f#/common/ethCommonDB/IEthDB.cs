using ethCommonDB.models;

using Microsoft.EntityFrameworkCore;

namespace ethCommonDB
{
    public interface IEthDB
    {
        public DbSet<TriggerHistory> triggerHistoriesEntities { get; set; }
        public DbSet<TokenInfo> tokenInfoEntities { get; set; }
        public DbSet<BlocksEntity> blocksEntities { get; set; }
        public DbSet<SwapsETH_USD> swapsETH_USDEntities { get; set; }
        public DbSet<SwapsETH_Token> swapsETH_TokenEntities { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
