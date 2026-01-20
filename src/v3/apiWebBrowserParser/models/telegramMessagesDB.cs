using Microsoft.EntityFrameworkCore;

namespace apiWebBrowserParser.models;

public class telegramMessagesDB(DbContextOptions<telegramMessagesDB> options) : DbContext(options)
{
    public required DbSet<messagesEntity> messagesEntities { get; set; }

    public DbSet<ApplicationActivation> ApplicationActivations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationActivation>()
            .HasMany(a => a.Messages)
            .WithOne(m => m.Activation)
            .HasForeignKey(m => m.UserKey)
            .HasPrincipalKey(a => a.UserKey);
    }
}
