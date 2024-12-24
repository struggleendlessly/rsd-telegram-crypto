namespace apiWebBrowserParser.models
{
    public class messagesEntity
    {
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;
        public string ChatName { get; set; } = string.Empty;

        public bool isSent { get; set; }
    }
}
