namespace Shared.ConfigurationOptions
{
    public class OptionsBaseScan
    {
        public static string SectionName { get; } = "BaseScan";
        public string baseUrl { get; set; }
        public string apiKeyToken { get; set; }
        public string apiKeyToken1 { get; set; }
        public string apiKeyToken2 { get; set; }
        public string UrlBasescanOrgAddress { get; set; }
        public string UrlBasescanOrgToken { get; set; }
        public string UrlDexscreenerComBase { get; set; }
    }
}