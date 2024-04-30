using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class DBModel : DbContext
    {
        public DbSet<TokenInfo> TokenInfos { get; set; }
        //public DbSet<TokenInfoUrl> TokenInfoUrls { get; set; }

        public string DbPath { get; }

        public DBModel()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "blogging.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer($"Server=tcp:rsdsite.database.windows.net,1433;Initial Catalog=cryptofilter;Persist Security Info=False;User ID=rsdsite;Password=1waq!WAQ;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
    }

    public class TokenInfo
    {
        public int Id { get; set; }
        public string AddressToken { get; set; }
        public string AddressOwnersWallet { get; set; }
        public int TelegramMessageId { get; set; }
        public string TelegramMessage { get; set; }

        public string UrlToken { get; set; }
        public string UrlOwnersWallet { get; set; }
        public string UrlChart { get; set; }

        public DateTime TimeAdded { get; set; }
        public DateTime TimeUpdated { get; set; }

        //public List<TokenInfoUrl> TokenInfoUrls { get; } = new();
    }

    //public class TokenInfoUrl
    //{
    //    public int Id { get; set; }
    //    public string UrlToken { get; set; }
    //    public string UrlOwnersWallet { get; set; }
    //    public string UrlChart { get; set; }

    //    public TokenInfo TokenInfo { get; set; }
    //}
}
