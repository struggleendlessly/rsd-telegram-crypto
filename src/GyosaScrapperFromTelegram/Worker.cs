using HtmlAgilityPack;

using System.Reflection;

namespace GyosaScrapperFromTelegram
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
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

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    }
            //    await Task.Delay(1000, stoppingToken);
            //}
        }

        public List<Gyosa> ExtractInnerHtml(string html)
        {
            List<Gyosa> res = new();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Find the div with class 'text'
            var divNodesMessages = doc.DocumentNode.SelectNodes("//div[contains(@class, 'message default clearfix')]");

            foreach (var nodeMessage in divNodesMessages)
            {
                var docT = new HtmlDocument();
                Gyosa t = new();
                docT.LoadHtml(nodeMessage.InnerHtml);
                //var childNodesText = nodeMessage.SelectSingleNode("//div[@class='text']");
                var childNodesText = docT.DocumentNode.SelectSingleNode("//div[@class='text']");

                //var divNodesDate = nodeMessage.SelectSingleNode("//div[contains(@class, 'date details')]");
                var divNodesDate = docT.DocumentNode.SelectSingleNode("//div[contains(@class, 'pull_right date details')]");
                var TimePosted = string.Empty;

                if (divNodesDate is not null)
                {
                    TimePosted = divNodesDate.GetAttributeValue("title", string.Empty);
                }
                var strong = childNodesText.InnerHtml.Split("<br>");

                var MarketCap = strong[1].Split(":")[1].Replace('$', ' ').Trim();
                var TotalBribeAttempts = strong[4].Split(":")[1].Replace("ETH", "").Trim().Split(" | ");
                var SuccessfulBribes = strong[5].Split(":")[1].Replace("ETH", "").Trim().Split(" | ");
                var Controling = strong[6].Split(":")[1].Split("%")[0].Trim();


                docT.LoadHtml(strong[0]);
                var Title = docT.DocumentNode.SelectNodes("//a")[0].InnerHtml;

                docT.LoadHtml(strong[9]);
                var Code = docT.DocumentNode.SelectNodes("//code")[0].InnerHtml;

                t.Title = Title;
                t.CA = Code;
                t.MarketCap = MarketCap;
                t.TotalBribeAttempts = TotalBribeAttempts[0];
                t.SuccessfulBribes = SuccessfulBribes[0];
                t.TotalBribeAttemptsETH = TotalBribeAttempts[1];
                t.SuccessfulBribesETH = SuccessfulBribes[1];
                t.Controling = Controling;
                t.TimePosted = TimePosted;

                res.Add(t);
            }

            return res;
        }
    }

    public class Gyosa
    {
        public string Title { get; set; }
        public string MarketCap { get; set; }
        public string TotalBribeAttempts { get; set; }
        public string SuccessfulBribes { get; set; }
        public string TotalBribeAttemptsETH { get; set; }
        public string SuccessfulBribesETH { get; set; }
        public string Controling { get; set; }
        public string CA { get; set; }
        public string TimePosted { get; set; }
    }
}
