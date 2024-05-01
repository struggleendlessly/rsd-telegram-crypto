namespace Shared.ConfigurationOptions
{
    public class OptionsTelegram
    {
        public static string SectionName { get; } = "Telegram";
        public string api_id { get; set; }
        public string api_hash { get; set; }
        public string phone_number { get; set; }
        public string session_pathname { get; set; }
    }
}