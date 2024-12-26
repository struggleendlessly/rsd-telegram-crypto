using Microsoft.EntityFrameworkCore;

namespace apiWebBrowserParser.models
{
    public class telegramMessagesDB(DbContextOptions<telegramMessagesDB> options) : DbContext(options)
    {
        public required DbSet<messagesEntity> messagesEntities { get; set; }
    }
}
