namespace Shared.ConfigurationOptions
{
    public class OptionsAlchemy
    {
        public static string SectionName { get; } = "ApiAlchemy";

        public string UrlBaseLocalNode { get; set; } = "";
        public string UrlBase { get; set; } = "";
        public string UrlVersion { get; set; } = "v2";
        public string[] ApiKeys { get; set; } = [];
        public ChainName ChainNames { get; set; } = new ChainName();
    }

    public class ChainName
    {
        public string Etherium { get; set; } = "Etherium";
        public string Base { get; set; } = "Base";
    }
}