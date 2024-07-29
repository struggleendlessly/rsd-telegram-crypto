using System.Text;

namespace etherscan
{
    public class UrlBuider
    {
        public static string getSourceCode(
            string address, 
            string apiKeyToken)
        {
            var res = string.Empty;

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append("api/?module=contract");
            urlBuilder.Append("&action=getsourcecode");
            urlBuilder.Append($"&address={address}");
            urlBuilder.Append($"&apikey={apiKeyToken}");

            res = urlBuilder.ToString();

            return res;
        }
    }
}
