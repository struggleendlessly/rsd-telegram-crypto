using HtmlAgilityPack;

using Shared.DB;

using System.Reflection;

namespace GyosaScrapperFromTelegram
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DBContext dbContext;

        public Worker(ILogger<Worker> logger, DBContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var htmlFilesPath = Path.Combine(exePath, "FilesToScrap");
            var htmlFilesContent = new List<string>();

            // Read all files from the folder
            var files = Directory.GetFiles(htmlFilesPath);

            List<Gyosa> gyozaList = new();

            // Process each file
            foreach (var file in files)
            {
                var t = File.ReadAllText(file);
                htmlFilesContent.Add(t);
            }

            foreach (var html in htmlFilesContent)
            {
                var t = ExtractInnerHtml(html);
                gyozaList.AddRange(t);
            }

            var filtered = Filter(gyozaList);

            SaveToDB(filtered);
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    }
            //    await Task.Delay(1000, stoppingToken);
            //}
        }

        public async Task SaveToDB(List<Gyosa> val)
        {
            bool res = false;

            dbContext.Gyosas.AddRange(val);
            await dbContext.SaveChangesAsync();
            res = true;
        }
        public List<Gyosa> Filter(List<Gyosa> val)
        {
            //Market Cap(MC) below $82,000
            //Total Bribe Attempts(TBA) and Successful Bribes(SB) above 21 wallets
            //Total ETH for TBA and SB above 0.91 ETH
            //Token Control(TC) above 30 %

            List<Gyosa> res = new();

            foreach (var item in val)
            {
                if (item.MarketCap < 82_000 &&
                    item.SuccessfulBribes > 21 &&
                    item.SuccessfulBribes > 21 &&
                    item.SuccessfulBribesETH > 0.91 &&
                    item.TotalBribeAttemptsETH > 0.91 &&
                    item.Controling > 30.0
                    )
                {
                    res.Add(item);
                }
            }

            return res;
        }

        public List<Gyosa> ExtractInnerHtml(string html)
        {
            List<Gyosa> res = new();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Find the div with class 'text'
            var divNodesMessages = doc.DocumentNode.SelectNodes("//div[contains(@class, 'message default clearfix')]");
            var c = 0;
            foreach (var nodeMessage in divNodesMessages)
            {
                var docT = new HtmlDocument();
                Gyosa t = new();
                docT.LoadHtml(nodeMessage.InnerHtml);

                var childNodesTitle = docT.DocumentNode.SelectSingleNode("//a");
                var Title = childNodesTitle.InnerHtml;

                var childNodesText = docT.DocumentNode.SelectSingleNode("//div[@class='text']");
                childNodesText.RemoveChild(childNodesTitle); // Remove the title from the text because as the name could be used big image !!!

                var divNodesDate = docT.DocumentNode.SelectSingleNode("//div[contains(@class, 'pull_right date details')]");
                var TimePosted = string.Empty;

                if (divNodesDate is not null)
                {
                    TimePosted = divNodesDate.GetAttributeValue("title", string.Empty);
                }

                var strong = childNodesText.InnerHtml.Split("<br>");

                try
                {
                    var MarketCap = strong[1].Split(":")[1].Replace('$', ' ').Replace(",", "").Trim();
                    var TotalBribeAttempts = strong[4].Split(":")[1].Replace("ETH", "").Trim().Split(" | ");
                    var SuccessfulBribes = strong[5].Split(":")[1].Replace("ETH", "").Trim().Split(" | ");
                    var Controling = strong[6].Split(":")[1].Split("%")[0].Trim().Replace(".", ",");


                    if (strong[4].Contains("strong"))
                    {
                        docT.LoadHtml(strong[4]);
                        var tt = docT.DocumentNode.InnerText;
                        TotalBribeAttempts = tt.Split(":")[1].Replace("ETH", "").Trim().Split(" | ");
                    }

                    if (strong[5].Contains("strong"))
                    {
                        docT.LoadHtml(strong[5]);
                        var tt = docT.DocumentNode.InnerText;
                        SuccessfulBribes = tt.Split(":")[1].Replace("ETH", "").Trim().Split(" | ");
                    }

                    if (MarketCap.Equals("nan", StringComparison.InvariantCultureIgnoreCase))
                    {
                        MarketCap = "0";
                    }

                    docT.LoadHtml(strong[9]);
                    var Code = docT.DocumentNode.SelectNodes("//code")[0].InnerHtml;

                    t.Title = Title;
                    t.CA = Code;
                    t.MarketCap = decimal.Parse(MarketCap);
                    t.TotalBribeAttempts = int.Parse(TotalBribeAttempts[0]);
                    t.SuccessfulBribes = int.Parse(SuccessfulBribes[0]);
                    t.TotalBribeAttemptsETH = double.Parse(TotalBribeAttempts[1].Replace(".", ","));
                    t.SuccessfulBribesETH = double.Parse(SuccessfulBribes[1].Replace(".", ","));
                    t.Controling = double.Parse(Controling);
                    t.TimePosted = TimePosted;

                    res.Add(t);
                    c++;
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            return res;
        }
    }
}
