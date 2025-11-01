using Puppeteer.Console.Services;

var localStorageSingleChatTelegramRunner = new LocalStorageSingleChatTelegramRunner();
var profileSingleChatTelegramRunner = new ProfileSingleChatTelegramRunner();

//await localStorageSingleChatTelegramRunner.RunAsync();
await profileSingleChatTelegramRunner.RunAsync();
