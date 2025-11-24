namespace Puppeteer.Console.BlazorUI;

public class AppStateManager
{
    public bool SaveLoginData { get; protected set; }

    public void SetSaveLoginData(string userInput)
    {
        if (!string.IsNullOrEmpty(userInput) && userInput.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            SaveLoginData = true;
    }
}
