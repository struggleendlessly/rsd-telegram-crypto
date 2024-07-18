namespace Shared.ConfigurationOptions
{
    public class OptionsAlchemy
    {
        public string UrlBase { get; set; }
        public string UrlVersion { get; set; }
        public string[] ApiKeys { get; set; }
        public Chainname[] ChainNames { get; set; }
    }

    public class Chainname
    {
        public string Etherium { get; set; }
        public string Base { get; set; }
    }
}