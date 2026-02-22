namespace apiWebBrowserParser.models;

public class ApplicationActivation
{
    public int Id { get; set; }

    public string UserKey { get; set; } = string.Empty;

    public string ChatId { get; set; } = string.Empty;

    public DateTime ActivatedAt { get; set; }

    public ICollection<messagesEntity> Messages { get; set; } = [];
}
