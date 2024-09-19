namespace Shared.ConfigurationOptions
{
    public class OptionsTelegram
    {
        public static string SectionName { get; } = "Telegram";
        public string UrlBase { get; set; }
        public int api_limit { get; set; }
        public int api_delay_worker { get; set; }
        public int api_delay_forech { get; set; }
        public string bot_hash { get; set; } = "";
        public string chat_id_coins { get; set; } = "";
        public string message_thread_id_mainfilters { get; set; } = "";
        public string message_thread_id_healthCheck { get; set; } = "";
        public string message_thread_id_botVerified { get; set; } = "";
        public string message_thread_id_unVerified { get; set; } = "";
        public string message_thread_id_p0 { get; set; } = "";
        public string message_thread_id_p10 { get; set; } = "";
        public string message_thread_id_p20_60mins { get; set; } = "";
        public string message_thread_id_p21_30mins { get; set; } = "";
        public string message_thread_id_p22_5mins { get; set; } = "";
        public string message_thread_id_p22_5_02mins { get; set; } = "";
        public string message_thread_id_p23_1mins { get; set; } = "";
        public string etherscanUrl { get; set; } = "";
        public string dextoolsUrl { get; set; } = "";
        public string dexscreenerUrl { get; set; } = "";
    }
}