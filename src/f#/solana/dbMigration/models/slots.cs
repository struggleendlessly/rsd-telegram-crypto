using Microsoft.EntityFrameworkCore;

namespace dbMigration.models
{
    [Index(nameof(numberInt), IsUnique = true)]
    public class slots
    {
        public int Id { get; set; }

        public UInt64 numberInt { get; set; } = 0;

    }
}
