using System.Text.Json;

namespace CryptChatV2.Services
{
    public class NgrokService
    {
        private const string NgrokExecutablePath = @"C:\ProgramData\chocolatey\bin\ngrok.exe";
        private const string NgrokApiUrl = "http://localhost:4040/api/tunnels";

        public static async Task<string> GetNgrokUrlAsync()
        {
            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = NgrokExecutablePath,
                    Arguments = "http https://localhost:7013"
                }
            };
            process.Start();
            await Task.Delay(5000);

            using var client = new HttpClient();
            var ngrokStatusResponse = await client.GetStringAsync(NgrokApiUrl);

            using (JsonDocument doc = JsonDocument.Parse(ngrokStatusResponse))
            {
                return doc.RootElement
                    .GetProperty("tunnels")[0]
                    .GetProperty("public_url")
                    .GetString();
            }
        }
    }
}