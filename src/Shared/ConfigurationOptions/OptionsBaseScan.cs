namespace Shared.ConfigurationOptions
{
    public class OptionsBaseScan
    {
        public static string SectionName { get; } = "BaseScan";
        public string baseUrl { get; set; }
        public string apiKeyToken { get; set; }
    }
}