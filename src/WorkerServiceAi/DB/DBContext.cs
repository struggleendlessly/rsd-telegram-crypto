using Microsoft.EntityFrameworkCore;

namespace WorkerServiceAi.DB
{
    public class DBContext : DbContext
    {
        public DbSet<Learn22> Learn22 { get; set; }

        public string DbPath { get; }

        public DBContext(DbContextOptions<DBContext> options)
        : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlServer($"Server=ARIANAGRANDE;Database=ai;Persist Security Info=False;User ID=sa;Password=1waq!WAQ1waq!WAQ;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=3;");
    }
}
