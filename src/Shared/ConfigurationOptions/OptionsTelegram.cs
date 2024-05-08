namespace Shared.ConfigurationOptions
{
    public class OptionsTelegram
    {
        public static string SectionName { get; } = "Telegram";
        public int api_limit { get; set; }
        public int api_delay_worker { get; set; }
        public int api_delay_forech { get; set; }
        public string bot_hash { get; set; } = "";
        public string chat_id_coins { get; set; } ="";
        public string message_thread_id_mainfilters { get; set; } ="";
    }
}