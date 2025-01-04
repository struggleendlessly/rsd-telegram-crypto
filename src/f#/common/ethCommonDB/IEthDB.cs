using ethCommonDB.models;

using Microsoft.EntityFrameworkCore;

namespace ethCommonDB
{
    public interface IEthDB
    {
        public  DbSet<TokenInfo> EthTokenInfoEntities { get; set; }
        public  DbSet<BlocksEntity> EthBlocksEntities { get; set; }
        public  DbSet<SwapsETH_USD> EthSwapsETH_USDEntities { get; set; }
        public  DbSet<SwapsETH_Token> EthSwapsETH_TokenEntities { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
