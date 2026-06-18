namespace Puppeteer.CryptoChat.Responses;

public record ApplicationActivationResponseDto(
    int Id,
    string UserKey, 
    DateTime ActivatedAt);
