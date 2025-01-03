using ethCommonDB.models;

using Microsoft.EntityFrameworkCore;

namespace ethCommonDB
{
    public interface IEthDB
    {
        public  DbSet<EthTokenInfo> EthTokenInfoEntities { get; set; }
        public  DbSet<EthBlocksEntity> EthBlocksEntities { get; set; }
        public  DbSet<EthSwapsETH_USD> EthSwapsETH_USDEntities { get; set; }
        public  DbSet<EthSwapsETH_Token> EthSwapsETH_TokenEntities { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
