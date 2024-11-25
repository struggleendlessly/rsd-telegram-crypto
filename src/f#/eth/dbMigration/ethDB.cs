using dbMigration.models;

using Microsoft.EntityFrameworkCore;

namespace dbMigration
{
    public class ethDB(DbContextOptions<ethDB> options) : DbContext(options)
    {
        public DbSet<MyEntity> MyEntity { get; set; }


        public string DbPath { get; }
    }
}
