using Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class dbContext : DbContext
    {
        public DbSet<EthBlocks> EthBlock { get; set; }
        public DbSet<EthTrxOthers> EthTrxOther { get; set; }
        public DbSet<EthTrainData> EthTrainData { get; set; }
        public DbSet<EthSwapEvents> EthSwapEvents { get; set; }

        public string DbPath { get; }

        public dbContext(DbContextOptions<dbContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer($"Server=192.168.0.70,1433;Initial Catalog=crypta;Persist Security Info=False;User ID=n100Sql;Password=5bgv%BGV5bgv%BGV;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;");

    }
}
