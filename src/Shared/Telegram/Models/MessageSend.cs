namespace Shared.Telegram.Models
{
    public class MessageSend
    {
        public bool ok { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public int message_id { get; set; }
        public From from { get; set; }
        public Chat chat { get; set; }
        public int date { get; set; }
        public int message_thread_id { get; set; }
        public Reply_To_Message reply_to_message { get; set; }
        public string text { get; set; }
        public Entity[] entities { get; set; }
        public Link_Preview_Options link_preview_options { get; set; }
        public bool is_topic_message { get; set; }
    }

    public class From
    {
        public long id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
    }

    public class Chat
    {
        public long id { get; set; }
        public string title { get; set; }
        public bool is_forum { get; set; }
        public string type { get; set; }
    }

    public class Reply_To_Message
    {
        public int message_id { get; set; }
        public From1 from { get; set; }
        public Chat1 chat { get; set; }
        public int date { get; set; }
        public int message_thread_id { get; set; }
        public Forum_Topic_Created forum_topic_created { get; set; }
        public bool is_topic_message { get; set; }
    }

    public class From1
    {
        public int id { get; set; }
        public bool is_bot { get; set; }
        public string first_name { get; set; }
        public string username { get; set; }
        public string language_code { get; set; }
        public bool is_premium { get; set; }
    }

    public class Chat1
    {
        public long id { get; set; }
        public string title { get; set; }
        public bool is_forum { get; set; }
        public string type { get; set; }
    }

    public class Forum_Topic_Created
    {
        public string name { get; set; }
        public int icon_color { get; set; }
    }

    public class Link_Preview_Options
    {
        public bool is_disabled { get; set; }
    }

    public class Entity
    {
        public int offset { get; set; }
        public int length { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }
}
