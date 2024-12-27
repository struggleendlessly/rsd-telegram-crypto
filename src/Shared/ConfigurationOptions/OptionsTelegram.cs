namespace Shared.ConfigurationOptions
{
    public class OptionsTelegram
    {
        public static string SectionName { get; } = "Telegram";
        public string UrlBase { get; set; }
        public int api_limit { get; set; }
        public int api_delay_worker { get; set; }
        public int api_delay_forech { get; set; }
        public List<string> bot_hash { get; set; } = [];
        public string chat_id_coins { get; set; } = "";

        
        public string message_thread_id_eth_more1m_less10m { get; set; } = "";
        public string message_thread_id_solana_more1m_less10m { get; set; } = "";
        public string message_thread_id_eth_more300k_less1m { get; set; } = "";
        public string message_thread_id_solana_more300k_less1m { get; set; } = "";
        public string message_thread_id_eth_more100k_less300k { get; set; } = "";
        public string message_thread_id_solana_more100k_less300k { get; set; } = "";
        public string message_thread_id_eth_less100k { get; set; } = "";
        public string message_thread_id_solana_less100k { get; set; } = "";

        public string message_thread_id_mainfilters { get; set; } = "";
        public string message_thread_id_healthCheck { get; set; } = "";
        public string message_thread_id_botVerified { get; set; } = "";
        public string message_thread_id_unVerified { get; set; } = "";
        public string message_thread_id_p0 { get; set; } = "";
        public string message_thread_id_p10 { get; set; } = "";
        public string message_thread_id_webscrapper { get; set; } = "";
        public string message_thread_id_p20_60mins { get; set; } = "";
        public string message_thread_id_p21_30mins { get; set; } = "";
        public string message_thread_id_p22_5mins { get; set; } = "";
        public string message_thread_id_p22_5_02mins { get; set; } = "";
        public string message_thread_id_p23_1mins { get; set; } = "";
        public string message_thread_id_p25_5mins_v03_mc0to100k { get; set; } = "";
        public string message_thread_id_p26_5mins_v09_mc100kto1m { get; set; } = "";
        public string message_thread_id_p212_30mins_v03_mc0to100k { get; set; } = "";
        public string message_thread_id_p28_30mins_v09_mc100kto1m { get; set; } = "";

        public string closed_chat_id_coins { get; set; } = "";
        public string closed_message_thread_id_p0_newTokens { get; set; } = "";
        public string closed_message_thread_id_p10_livePairs { get; set; } = "";
        public string closed_message_thread_id_p20_60mins { get; set; } = "";
        public string closed_message_thread_id_p21_30mins { get; set; } = "";
        public string closed_message_thread_id_p22_5mins { get; set; } = "";
        public string closed_message_thread_id_p22_5_02mins { get; set; } = "";
        public string closed_message_thread_id_p23_1mins { get; set; } = "";
        public string closed_message_thread_id_p25_5mins_v03_mc0to100k { get; set; } = "";
        public string closed_message_thread_id_p26_5mins_v09_mc100kto1m { get; set; } = "";
        public string closed_message_thread_id_p212_30mins_v03_mc0to100k { get; set; } = "";
        public string closed_message_thread_id_p28_30mins_v09_mc100kto1m { get; set; } = "";

        public string public_chat_id_coins { get; set; } = "";
        public string public_message_thread_id_p0_newTokens { get; set; } = "";
        public string public_message_thread_id_p10_livePairs { get; set; } = "";
        public string public_message_thread_id_p20_60mins { get; set; } = "";
        public string public_message_thread_id_p21_30mins { get; set; } = "";
        public string public_message_thread_id_p22_5mins { get; set; } = "";
        public string public_message_thread_id_p22_5_02mins { get; set; } = "";
        public string public_message_thread_id_p23_1mins { get; set; } = "";
        public string public_message_thread_id_p25_5mins_v03_mc0to100k { get; set; } = "";
        public string public_message_thread_id_p26_5mins_v09_mc100kto1m { get; set; } = "";
        public string public_message_thread_id_p212_30mins_v03_mc0to100k { get; set; } = "";
        public string public_message_thread_id_p28_30mins_v09_mc100kto1m { get; set; } = "";

        public string etherscanUrl { get; set; } = "";
        public string dextoolsUrl { get; set; } = "";
        public string dexscreenerUrl { get; set; } = "";
    }
}