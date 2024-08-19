using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;

using Newtonsoft.Json;

using Shared.ConfigurationOptions;

using System.Numerics;

namespace nethereum
{
    public class ApiWeb3
    {
        private readonly OptionsAlchemy optionsAlchemy;
        private string vAndApiKey = string.Empty;
        private readonly ILogger<ApiWeb3> logger;

        string pairAbi = @"[{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""owner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""spender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Burn"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""name"":""Mint"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Swap"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""uint112"",""name"":""reserve0"",""type"":""uint112""},{""indexed"":false,""internalType"":""uint112"",""name"":""reserve1"",""type"":""uint112""}],""name"":""Sync"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""from"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""constant"":true,""inputs"":[],""name"":""DOMAIN_SEPARATOR"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""MINIMUM_LIQUIDITY"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""PERMIT_TYPEHASH"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""allowance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""approve"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""burn"",""outputs"":[{""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""decimals"",""outputs"":[{""internalType"":""uint8"",""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""factory"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getReserves"",""outputs"":[{""internalType"":""uint112"",""name"":""_reserve0"",""type"":""uint112""},{""internalType"":""uint112"",""name"":""_reserve1"",""type"":""uint112""},{""internalType"":""uint32"",""name"":""_blockTimestampLast"",""type"":""uint32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""_token0"",""type"":""address""},{""internalType"":""address"",""name"":""_token1"",""type"":""address""}],""name"":""initialize"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""kLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""mint"",""outputs"":[{""internalType"":""uint256"",""name"":""liquidity"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""nonces"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""owner"",""type"":""address""},{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""deadline"",""type"":""uint256""},{""internalType"":""uint8"",""name"":""v"",""type"":""uint8""},{""internalType"":""bytes32"",""name"":""r"",""type"":""bytes32""},{""internalType"":""bytes32"",""name"":""s"",""type"":""bytes32""}],""name"":""permit"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price0CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price1CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""skim"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""bytes"",""name"":""data"",""type"":""bytes""}],""name"":""swap"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""sync"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token0"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token1"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""from"",""type"":""address""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";
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

        public List<EventLog<List<ParameterOutput>>> DecodeSwapEvents(string logsJson)
        {
            List<EventLog<List<ParameterOutput>>> res = new();

            var pairAbiWithSwap = @"[{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""owner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""spender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Burn"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""name"":""Mint"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Swap"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""uint112"",""name"":""reserve0"",""type"":""uint112""},{""indexed"":false,""internalType"":""uint112"",""name"":""reserve1"",""type"":""uint112""}],""name"":""Sync"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""from"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""constant"":true,""inputs"":[],""name"":""DOMAIN_SEPARATOR"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""MINIMUM_LIQUIDITY"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""PERMIT_TYPEHASH"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""allowance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""approve"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""burn"",""outputs"":[{""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""decimals"",""outputs"":[{""internalType"":""uint8"",""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""factory"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getReserves"",""outputs"":[{""internalType"":""uint112"",""name"":""_reserve0"",""type"":""uint112""},{""internalType"":""uint112"",""name"":""_reserve1"",""type"":""uint112""},{""internalType"":""uint32"",""name"":""_blockTimestampLast"",""type"":""uint32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""_token0"",""type"":""address""},{""internalType"":""address"",""name"":""_token1"",""type"":""address""}],""name"":""initialize"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""kLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""mint"",""outputs"":[{""internalType"":""uint256"",""name"":""liquidity"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""nonces"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""owner"",""type"":""address""},{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""deadline"",""type"":""uint256""},{""internalType"":""uint8"",""name"":""v"",""type"":""uint8""},{""internalType"":""bytes32"",""name"":""r"",""type"":""bytes32""},{""internalType"":""bytes32"",""name"":""s"",""type"":""bytes32""}],""name"":""permit"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price0CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price1CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""skim"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""bytes"",""name"":""data"",""type"":""bytes""}],""name"":""swap"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""sync"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token0"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token1"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""from"",""type"":""address""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";
            var pairContract = web3.Eth.GetContract(pairAbiWithSwap, "");

            FilterLog[] logs = JsonConvert.DeserializeObject<FilterLog[]>(logsJson);

            var decoded = pairContract.
                            ContractBuilder.
                            GetEventAbi("Swap").
                            DecodeAllEventsDefaultTopics(logs);

            res = decoded;

            return res;
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

        public string DecodeLiquidityInput(string function, string input)
        {
            var res = string.Empty;

            var param = buildInputParameters(function);

            if (param.functionName is null)
            {
                return res;
            }

            var functionABI = new FunctionABI(param.functionName, false);


            functionABI.InputParameters = param.Params;

            List<ParameterOutput> decoded = new();

            try
            {
                decoded = functionABI.DecodeInputDataToDefault(input);
            }
            catch (Exception ex)
            {
                logger.LogWarning("DecodeInputDataToDefault {input}", input);
                logger.LogWarning("DecodeInputDataToDefault {mes}", ex.Message);
            }

            if (decoded is null ||
                decoded.Count == 0)
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
                //logger.LogInformation("new Parameter values {t}", t);

                if (t[1].Contains("token", StringComparison.InvariantCultureIgnoreCase) &&
                    t[0].Contains("address", StringComparison.InvariantCultureIgnoreCase))
                {
                    tokenIndex = i;
                }

                res.Add(new Parameter(t[0], t[1], i + 1));

            }




            return (functionName, tokenIndex, res.ToArray());
        }

        public async Task<string> PerformEthCall(
            string functionName,
            string contractAddress)
        {
            var contract = web3.Eth.GetContract(pairAbi, contractAddress);

            var functionSignature =
                contract.
                ContractBuilder.
                ContractABI.
                Functions.
                Where(x => x.Name.Equals(functionName, StringComparison.InvariantCultureIgnoreCase)).
                FirstOrDefault().
                Sha3Signature;

            var callInput = new CallInput
            {
                //From = "0x88e6a0c2ddd26feeb64f039a2c41296fcb3f5640",
                To = contractAddress,
                Data = functionSignature,
                //Gas = new HexBigInteger(500000),
                //GasPrice = new HexBigInteger(0)
            };

            var result = await web3.Eth.Transactions.Call.SendRequestAsync(callInput);

            return result;
        }
    }
}
