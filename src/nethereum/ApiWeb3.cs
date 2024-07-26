using Microsoft.Extensions.Options;

using Nethereum.Hex.HexTypes;
using Nethereum.Web3;

using Shared.ConfigurationOptions;

using System.Numerics;

namespace nethereum
{
    public class ApiWeb3
    {
        private readonly OptionsAlchemy optionsAlchemy;
        private string vAndApiKey = string.Empty;

        Web3 web3 = new Web3();
        public ApiWeb3(
            IOptions<OptionsAlchemy> _optionsAlchemy)
        {
            optionsAlchemy = _optionsAlchemy.Value;
            var url = optionsAlchemy.UrlBase.Replace("{{{chainName}}}", optionsAlchemy.ChainNames.Etherium);

            Random rnd = new Random();
            var apiKeyIndex = rnd.Next(0, optionsAlchemy.ApiKeys.Length - 1);
            vAndApiKey = $"/v2/{optionsAlchemy.ApiKeys[apiKeyIndex]}";
            var fullUrl = url + vAndApiKey;

            web3 = new Web3(fullUrl);
        }

        // var totalSupplySelector = "0x18160ddd";
        public async Task<BigInteger> GetTotalSupply(
            string tokenAddress, 
            string totalSupplySelector = "0x18160ddd")
        {
            // Create a call input with the function selector for totalSupply()
            var callInput = new Nethereum.RPC.Eth.DTOs.CallInput
            {
                To = tokenAddress,
                Data = totalSupplySelector
            };

            // Send the call and get the result
            var result = await web3.Eth.Transactions.Call.SendRequestAsync(callInput);

            // Convert the result from hex to BigInteger
            var totalSupply = new HexBigInteger(result).Value;
            return totalSupply;
        }
    }
}
