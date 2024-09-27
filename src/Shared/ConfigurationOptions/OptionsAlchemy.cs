namespace Shared.ConfigurationOptions
{
    public class OptionsEtherscan
    {
        public static string SectionName { get; } = "ApiEtherscan";

        public string UrlBase { get; set; } = "";
        
        public string[] ApiKeys { get; set; } = [];
    }
}