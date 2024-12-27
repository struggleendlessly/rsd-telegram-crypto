namespace apiWebBrowserParser.models
{
    public class messagesEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public double MK { get; set; }
        public string Address { get; set; } = string.Empty;

        public bool isSolana { get; set; }
        public bool isBase { get; set; }
        public bool isETH { get; set; }

        public bool isSent { get; set; }
    }
}
