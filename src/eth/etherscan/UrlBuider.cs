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
        
        public static string getNormalTxn(
            string ownerAddress, 
            string apiKeyToken, 
            int page = 1)
        {
            var res = string.Empty;

            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append("api/?module=account");
            urlBuilder.Append("&action=txlist");
            urlBuilder.Append($"&address={ownerAddress}");
            urlBuilder.Append($"&startblock=0");
            urlBuilder.Append($"&endblock=99999999");
            urlBuilder.Append($"&startblock=0");
            urlBuilder.Append($"&page={page}");
            urlBuilder.Append("&offset=999");
            urlBuilder.Append("&sort=asc");
            urlBuilder.Append($"&apikey={apiKeyToken}");

            res = urlBuilder.ToString();

            return res;
        }
    }
}
