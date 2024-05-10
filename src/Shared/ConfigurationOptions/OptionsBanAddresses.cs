namespace Shared.ConfigurationOptions
{
    public class OptionsBanAddresses
    {
        public static string SectionName { get; } = "BanAddresses";
        public List<string> Addresses { get; set; } = new List<string>();

    }
}