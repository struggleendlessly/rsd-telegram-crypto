using Microsoft.EntityFrameworkCore;
using Puppeteer.Console.BlazorUI.Models;

namespace Puppeteer.Console.BlazorUI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public virtual DbSet<TelegramChat> TelegramChats { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TelegramChat>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).ValueGeneratedOnAdd();
            entity.Property(x => x.Url).IsRequired().HasMaxLength(500);
            entity.Property(x => x.Active).IsRequired();
        });
    }
}
