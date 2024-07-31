using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
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
        private readonly ILogger<ApiWeb3> logger;

        Web3 web3 = new Web3();
        public ApiWeb3(
            ILogger<ApiWeb3> logger,
            IOptions<OptionsAlchemy> _optionsAlchemy)
        {
            this.logger = logger;
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

        public string DecodeAddLiquidityInput(string function, string input)
        {
            var res = string.Empty;

            var param = buildInputParameters(function);

            if (param.functionName is null)
            {
                return res;
            }

            var functionABI = new FunctionABI(param.functionName, false);


            functionABI.InputParameters = param.Params;

            var decoded = functionABI.DecodeInputDataToDefault(input);

            if (decoded is null)
            {
                logger.LogWarning("DecodeAddLiquidityInput: decoded is null with function name: {function} and input {input}", function, input);
                return res;
            }

            var tokenAddress = decoded[param.TokenIndex].Result.ToString();

            if (!string.IsNullOrEmpty(tokenAddress))
            {
                res = tokenAddress;
            }

            return res;
        }

        //addLiquidityETH(address token, uint256 amountTokenDesired, uint256 amountTokenMin, uint256 amountETHMin, address to, uint256 deadline)
        private (string functionName, int TokenIndex, Parameter[] Params) buildInputParameters(string function)
        {
            List<Parameter> res = [];
            var tokenIndex = 0;

            var functionName = function.Split("(")[0];
            var str = function.Split("(")[1].Split(")")[0].Split(",");

            if (str.Length == 0 ||
                (str.Length == 1 &&
                string.IsNullOrEmpty(str[0]))
                )
            {
                return (null, 0, null);
            }

            for (int i = 0; i < str.Length; i++)
            {
                var t = str[i].Trim().Split(" ");
                Console.WriteLine(t);
                try
                {
                    if (t[1].Contains("token", StringComparison.InvariantCultureIgnoreCase) &&
                        t[0].Contains("address", StringComparison.InvariantCultureIgnoreCase))
                    {
                        tokenIndex = i;
                    }

                }
                catch (Exception ex)
                {

                    throw;
                }

                res.Add(new Parameter(t[0], t[1], i + 1));

            }

            return (functionName, tokenIndex, res.ToArray());
        }
    }
}
