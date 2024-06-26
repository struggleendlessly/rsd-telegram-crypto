using Microsoft.EntityFrameworkCore;

namespace Shared.DB
{
    public class DBContext : DbContext
    {
        public DbSet<TokenInfo> TokenInfos { get; set; }
        public DbSet<ContractSourceCode> ContractSourceCodes { get; set; }

        public string DbPath { get; }

        public DBContext(DbContextOptions<DBContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer($"Server=tcp:rsdsite.database.windows.net,1433;Initial Catalog=cryptofilter;Persist Security Info=False;User ID=rsdsite;Password=1waq!WAQ;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
    }
}
