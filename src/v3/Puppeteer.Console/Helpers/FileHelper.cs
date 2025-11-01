namespace Puppeteer.Console.Helpers;

public static class FileHelper
{
    public static async Task<bool> SessionFilesExist(string localStorageFilePath, string cookiesFilePath) =>
        await Task.Run(() => File.Exists(localStorageFilePath) && File.Exists(cookiesFilePath));
}
