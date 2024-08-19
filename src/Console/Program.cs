// See https://aka.ms/new-console-template for more information
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.ABI.Model;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Net.Mail;
using System.Numerics;
using System.Text.Json;

using TL;

//await Program1.Main();
var eee = new JsonRpcClient("https://eth-mainnet.g.alchemy.com");
var ddd = await eee.SendRequest();
var abi = """
[{"inputs":[],"payable":false,"stateMutability":"nonpayable","type":"constructor"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"owner","type":"address"},{"indexed":true,"internalType":"address","name":"spender","type":"address"},{"indexed":false,"internalType":"uint256","name":"value","type":"uint256"}],"name":"Approval","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"sender","type":"address"},{"indexed":false,"internalType":"uint256","name":"amount0","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount1","type":"uint256"},{"indexed":true,"internalType":"address","name":"to","type":"address"}],"name":"Burn","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"sender","type":"address"},{"indexed":false,"internalType":"uint256","name":"amount0","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount1","type":"uint256"}],"name":"Mint","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"sender","type":"address"},{"indexed":false,"internalType":"uint256","name":"amount0In","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount1In","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount0Out","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount1Out","type":"uint256"},{"indexed":true,"internalType":"address","name":"to","type":"address"}],"name":"Swap","type":"event"},{"anonymous":false,"inputs":[{"indexed":false,"internalType":"uint112","name":"reserve0","type":"uint112"},{"indexed":false,"internalType":"uint112","name":"reserve1","type":"uint112"}],"name":"Sync","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"from","type":"address"},{"indexed":true,"internalType":"address","name":"to","type":"address"},{"indexed":false,"internalType":"uint256","name":"value","type":"uint256"}],"name":"Transfer","type":"event"},{"constant":true,"inputs":[],"name":"DOMAIN_SEPARATOR","outputs":[{"internalType":"bytes32","name":"","type":"bytes32"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"MINIMUM_LIQUIDITY","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"PERMIT_TYPEHASH","outputs":[{"internalType":"bytes32","name":"","type":"bytes32"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"internalType":"address","name":"","type":"address"},{"internalType":"address","name":"","type":"address"}],"name":"allowance","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"spender","type":"address"},{"internalType":"uint256","name":"value","type":"uint256"}],"name":"approve","outputs":[{"internalType":"bool","name":"","type":"bool"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"internalType":"address","name":"","type":"address"}],"name":"balanceOf","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"to","type":"address"}],"name":"burn","outputs":[{"internalType":"uint256","name":"amount0","type":"uint256"},{"internalType":"uint256","name":"amount1","type":"uint256"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"decimals","outputs":[{"internalType":"uint8","name":"","type":"uint8"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"factory","outputs":[{"internalType":"address","name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getReserves","outputs":[{"internalType":"uint112","name":"_reserve0","type":"uint112"},{"internalType":"uint112","name":"_reserve1","type":"uint112"},{"internalType":"uint32","name":"_blockTimestampLast","type":"uint32"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"_token0","type":"address"},{"internalType":"address","name":"_token1","type":"address"}],"name":"initialize","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"kLast","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"to","type":"address"}],"name":"mint","outputs":[{"internalType":"uint256","name":"liquidity","type":"uint256"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"name","outputs":[{"internalType":"string","name":"","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"internalType":"address","name":"","type":"address"}],"name":"nonces","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"owner","type":"address"},{"internalType":"address","name":"spender","type":"address"},{"internalType":"uint256","name":"value","type":"uint256"},{"internalType":"uint256","name":"deadline","type":"uint256"},{"internalType":"uint8","name":"v","type":"uint8"},{"internalType":"bytes32","name":"r","type":"bytes32"},{"internalType":"bytes32","name":"s","type":"bytes32"}],"name":"permit","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"price0CumulativeLast","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"price1CumulativeLast","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"to","type":"address"}],"name":"skim","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"internalType":"uint256","name":"amount0Out","type":"uint256"},{"internalType":"uint256","name":"amount1Out","type":"uint256"},{"internalType":"address","name":"to","type":"address"},{"internalType":"bytes","name":"data","type":"bytes"}],"name":"swap","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"symbol","outputs":[{"internalType":"string","name":"","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"sync","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"token0","outputs":[{"internalType":"address","name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"token1","outputs":[{"internalType":"address","name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"totalSupply","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"to","type":"address"},{"internalType":"uint256","name":"value","type":"uint256"}],"name":"transfer","outputs":[{"internalType":"bool","name":"","type":"bool"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"from","type":"address"},{"internalType":"address","name":"to","type":"address"},{"internalType":"uint256","name":"value","type":"uint256"}],"name":"transferFrom","outputs":[{"internalType":"bool","name":"","type":"bool"}],"payable":false,"stateMutability":"nonpayable","type":"function"}]
""";
var pairAbi = @"[{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""owner"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""spender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Burn"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""name"":""Mint"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""sender"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1In"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""indexed"":false,""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""Swap"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""internalType"":""uint112"",""name"":""reserve0"",""type"":""uint112""},{""indexed"":false,""internalType"":""uint112"",""name"":""reserve1"",""type"":""uint112""}],""name"":""Sync"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""internalType"":""address"",""name"":""from"",""type"":""address""},{""indexed"":true,""internalType"":""address"",""name"":""to"",""type"":""address""},{""indexed"":false,""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""constant"":true,""inputs"":[],""name"":""DOMAIN_SEPARATOR"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""MINIMUM_LIQUIDITY"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""PERMIT_TYPEHASH"",""outputs"":[{""internalType"":""bytes32"",""name"":"""",""type"":""bytes32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""},{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""allowance"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""approve"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""burn"",""outputs"":[{""internalType"":""uint256"",""name"":""amount0"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""decimals"",""outputs"":[{""internalType"":""uint8"",""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""factory"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getReserves"",""outputs"":[{""internalType"":""uint112"",""name"":""_reserve0"",""type"":""uint112""},{""internalType"":""uint112"",""name"":""_reserve1"",""type"":""uint112""},{""internalType"":""uint32"",""name"":""_blockTimestampLast"",""type"":""uint32""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""_token0"",""type"":""address""},{""internalType"":""address"",""name"":""_token1"",""type"":""address""}],""name"":""initialize"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""kLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""mint"",""outputs"":[{""internalType"":""uint256"",""name"":""liquidity"",""type"":""uint256""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""name"":""nonces"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""owner"",""type"":""address""},{""internalType"":""address"",""name"":""spender"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""deadline"",""type"":""uint256""},{""internalType"":""uint8"",""name"":""v"",""type"":""uint8""},{""internalType"":""bytes32"",""name"":""r"",""type"":""bytes32""},{""internalType"":""bytes32"",""name"":""s"",""type"":""bytes32""}],""name"":""permit"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price0CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""price1CumulativeLast"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""}],""name"":""skim"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""uint256"",""name"":""amount0Out"",""type"":""uint256""},{""internalType"":""uint256"",""name"":""amount1Out"",""type"":""uint256""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""bytes"",""name"":""data"",""type"":""bytes""}],""name"":""swap"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""internalType"":""string"",""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""sync"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token0"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token1"",""outputs"":[{""internalType"":""address"",""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""internalType"":""uint256"",""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""internalType"":""address"",""name"":""from"",""type"":""address""},{""internalType"":""address"",""name"":""to"",""type"":""address""},{""internalType"":""uint256"",""name"":""value"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""internalType"":""bool"",""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""}]";

//var web3 = new Web3("https://eth-mainnet.g.alchemy.com/v2/3ex6KSnDnxa98_q_F6CD26ByLMK4Gga-");
await Main6();
async Task Main6()
{
    // Load an existing wallet using a private key
    string privateKey = "a1df9cd7d5d7e7943293f9e82f0789b5b4df7381e39aa3a0507d352b8b235b61"; // Replace with your private key
    var account = new Account(privateKey);

    // Connect to Ethereum network using Alchemy and the wallet
    var web3 = new Web3(account, "https://eth-mainnet.g.alchemy.com/v2/3ex6KSnDnxa98_q_F6CD26ByLMK4Gga-", null, null);

    // Now you can perform eth_call or other interactions
    var result = await PerformEthCall(web3);
    Console.WriteLine($"Result: {result}");
}

async Task<string> PerformEthCall(Web3 web3)
{
    // Example of using eth_call to get data from a smart contract
    string contractAddress = "0xb3014e8171155e90aa2d9ca995db069a89aabe06"; // Replace with your contract address
    string functionSignature = "0dfe1681";// "0x0902f1ac"; // Example: getReserves() function signature

    var contract = web3.Eth.GetContract(pairAbi, contractAddress);
    var function = contract.GetFunction("token1");
    var ee =
    contract.ContractBuilder.ContractABI.Functions.Where(x => x.Name.Equals("token1", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault().Sha3Signature;

    var callInput = new Nethereum.RPC.Eth.DTOs.CallInput
    {
        From = "0x88e6a0c2ddd26feeb64f039a2c41296fcb3f5640",
        To = contractAddress,
        Data = functionSignature,
        Gas = new HexBigInteger(500000),
        GasPrice = new HexBigInteger(0)
    };

    // Example: Call at the latest block
    var result = await web3.Eth.Transactions.Call.SendRequestAsync(callInput);

    return result;
}
string UniswapPairAbi = @"[
        {
            ""constant"": true,
            ""inputs"": [],
            ""name"": ""getReserves"",
            ""outputs"": [
                { ""name"": ""reserve0"", ""type"": ""uint112"" },
                { ""name"": ""reserve1"", ""type"": ""uint112"" },
                { ""name"": ""blockTimestampLast"", ""type"": ""uint32"" }
            ],
            ""payable"": false,
            ""stateMutability"": ""view"",
            ""type"": ""function""
        }
    ]";

//await Main5();

// Minimal ABI for Uniswap pair contract


//async Task Main5()
//{

//    string pairContractAddress = "0xc45a81bc23a64ea556ab4cdf08a86b61cdceea8b"; // Replace with actual pair contract address

//    // Example block number for historical data
//    ulong blockNumber = 20534300; // Replace with the actual historical block number

//    var reserves = await GetHistoricalReserves(web3, pairContractAddress, blockNumber);

//    Console.WriteLine($"Reserve0: {reserves.Reserve0}");
//    Console.WriteLine($"Reserve1: {reserves.Reserve1}");
//    Console.WriteLine($"Block Timestamp: {reserves.BlockTimestamp}");
//}

//async Task<Reserves> GetHistoricalReserves(Web3 web3, string pairContractAddress, ulong blockNumber)
//{
//    var contract = web3.Eth.GetContract(UniswapPairAbi, pairContractAddress);
//    var getReservesFunction = contract.GetFunction("getReserves");

//    var reserves = await getReservesFunction.CallDeserializingToObjectAsync<Reserves>(
//         new BlockParameter(blockNumber)
//    );

//    return reserves;
//}

public class Reserves
{
    [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("uint112", "reserve0", 1)]
    public BigInteger Reserve0 { get; set; }

    [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("uint112", "reserve1", 2)]
    public BigInteger Reserve1 { get; set; }

    [Nethereum.ABI.FunctionEncoding.Attributes.Parameter("uint32", "blockTimestampLast", 3)]
    public uint BlockTimestamp { get; set; }
}


//////////////////////////////
//async Task Main2()
//{

//    var contractAddress = "0x1e79d6529f271876d202cbb216b856165d862353";
//    var fromBlock = new BlockParameter(20530466); // Replace with the starting block number
//    var toBlock = BlockParameter.CreateLatest();

//    var abi = "UNISWAP_PAIR_CONTRACT_ABI";
//    var contract = web3.Eth.GetContract(pairAbi, contractAddress);
//    var logsss = """

//            {
//                "address": "0x1e79d6529f271876d202cbb216b856165d862353",
//                "topics": [
//                    "0x1c411e9a96e071241c2f21f7726b17ae89e3cab4c78be50e062b03a9fffbbad1"
//                ],
//                "data": "0x00000000000000000000000000000000000000000006f03960c53c41233236cc0000000000000000000000000000000000000000000000051be33065916b9571",
//                "blockNumber": "0x13950f2",
//                "transactionHash": "0x50016bf3e5c54e0710bc28ae417d159f8c018f2014855ff748acb820b8296bf5",
//                "transactionIndex": "0x10",
//                "blockHash": "0x4025d92a314463892674e277fe40c5e3831b56d30edf25361cf45f2ca8097477",
//                "logIndex": "0x57",
//                "removed": false
//            }

//            """;
//    var getReservesFunction = contract.GetFunction("getReserves");

//    // Define the function ABI
//    var functionABI = new FunctionABI("getReserves", false);

//    // Create input parameters
//    var parameters = new List<Parameter>
//        {
//            new Parameter("address", "recipient"),
//            new Parameter("uint256", "amount")
//        };

//    // Add parameters to the function ABI
//    //functionABI.InputParameters = parameters.ToArray();
//    //var dd = new HexBigInteger("0x0000000000000000000000000000000000000000000836198c1bd37245ece30f");
//    //var ee = getReservesFunction.DecodeInput(logsss);
//    //var blockNumber = new HexBigInteger("20530466"); // Replace with the actual block number from one month ago
//    //var reserves = await getReservesFunction.CallAsync<BigInteger>();
//    //var reserves5 = await getReservesFunction.CallAsync<Reserves>(blockNumber);

//    //Console.WriteLine($"Reserve0: {reserves.Reserve0}, Reserve1: {reserves.Reserve1}");

//    //var Topics = new[] { Event<SyncEventDTO>.GetEventABI().DecodeEvent() };
//    var filterInput = web3.Eth.GetEvent<SyncEventDTO>(contractAddress).CreateFilterInput(fromBlock = fromBlock, toBlock = toBlock );

//    var logs = await web3.Eth.Filters.GetLogs.SendRequestAsync(filterInput);

//    foreach (var log in logs)
//    {
//        var decodedEvent = Event<SyncEventDTO>.DecodeEvent(log);
//        if (decodedEvent != null)
//        {
//            Console.WriteLine($"Reserve0: {decodedEvent.Event.Reserve0}");
//            Console.WriteLine($"Reserve1: {decodedEvent.Event.Reserve1}");
//            Console.WriteLine($"BlockTimestampLast: {decodedEvent.Event.BlockTimestampLast}");
//        }
//    }
//}
//async Task Main1()
//{


//    // Replace with your Uniswap pair contract address
//    var pairAddress = "0x4103e658a8acab924a9a2d3750e6cf3fb932a186";
//    var FromBlock = new BlockParameter(20460858);
//    var ToBlock = new BlockParameter(20461858);
//    //await CalculateHistoricalPrices(web3, pairAddress, FromBlock, ToBlock);
//}
//  async Task Main()
//{
//    var pairContract = web3.Eth.GetContract(pairAbi, "0x1e79d6529f271876d202cbb216b856165d862353");

//    var logsss = """
//            [
//            {
//                "address": "0x1e79d6529f271876d202cbb216b856165d862353",
//                "topics": [
//                    "0x1c411e9a96e071241c2f21f7726b17ae89e3cab4c78be50e062b03a9fffbbad1"
//                ],
//                "data": "0x00000000000000000000000000000000000000000006f03960c53c41233236cc0000000000000000000000000000000000000000000000051be33065916b9571",
//                "blockNumber": "0x13950f2",
//                "transactionHash": "0x50016bf3e5c54e0710bc28ae417d159f8c018f2014855ff748acb820b8296bf5",
//                "transactionIndex": "0x10",
//                "blockHash": "0x4025d92a314463892674e277fe40c5e3831b56d30edf25361cf45f2ca8097477",
//                "logIndex": "0x57",
//                "removed": false
//            }
//            ]
//            """;
//    FilterLog[] ttt = JsonConvert.DeserializeObject<FilterLog[]>(logsss);

//    //var wl = pairContract..CallDecodingToDefaultAsync("0x00000000000000000000000000000000000000000006f03960c53c41233236cc0000000000000000000000000000000000000000000000051be33065916b9571");

//    var contractAddress = "0x1e79d6529f271876d202cbb216b856165d862353"; // Uniswap Pair contract address

//    var syncEvent = web3.Eth.GetEvent<SyncEventDTO>(contractAddress);

//    // Specify the block range for yesterday (replace with actual block numbers)
//    var blockNumberYesterdayStart = new BlockParameter(20530466);
//    var blockNumberYesterdayEnd = new BlockParameter(20533466);

//    var filterAll = syncEvent.CreateFilterInput(blockNumberYesterdayStart, blockNumberYesterdayEnd);
//    var logs = await syncEvent.GetAllChangesAsync(filterAll);

//    foreach (var log in logs)
//    {
//        Console.WriteLine($"Reserve0: {log.Event.Reserve0}, Reserve1: {log.Event.Reserve1}");
//    }
//}


//[Event("Sync")]
//public class SyncEventDTO : IEventDTO
//{
//    [Parameter("uint112", "_reserve0", 1)]
//    public BigInteger Reserve0 { get; set; }

//    [Parameter("uint112", "_reserve1", 2)]
//    public BigInteger Reserve1 { get; set; }

//    [Parameter("uint32", "_blockTimestampLast", 3)]
//    public uint BlockTimestampLast { get; set; }
//}

////async Task CalculateHistoricalPrices(Web3 web3, string pairAddress, BlockParameter fromBlock, BlockParameter toBlock)
////{
////    var pairContract = web3.Eth.GetContract(pairAbi, pairAddress);

////    // Fetch swap events
////    var swapEvent = pairContract.GetEvent("Swap");

////    var filter = swapEvent.CreateFilterInput(fromBlock, toBlock);
////    var swapEvents = await swapEvent.GetAllChangesDefaultAsync(filter);
////    var e = new EventABI("Swap");

////    var logsss = """
//        [
//        {
//            "address": "0x69c7bd26512f52bf6f76fab834140d13dda673ca",
//            "blockHash": "0x8c9fddd5694c82e843cb089693e89770c38e2612a9c13940247aa0f5e60f10d2",
//            "blockNumber": "0x138354c",
//            "data": "0x00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000f43fc2c04ee00000000000000000000000000000000000000000000000024cd39ebaec772bb23090000000000000000000000000000000000000000000000000000000000000000",
//            "logIndex": "0x113",
//            "removed": false,
//            "topics": [
//                "0xd78ad95fa46c994b6551d0da85fc275fe613ce37657fb8d5e3d130840159d822",
//                "0x0000000000000000000000003fc91a3afd70395cd496c647d5a6cc9d4b2b7fad",
//                "0x0000000000000000000000003fc91a3afd70395cd496c647d5a6cc9d4b2b7fad"
//            ],
//            "transactionHash": "0xa7b2b1d29a63d318c53e936af4244d57b80c9ce18b74f18c42f288e6580f7c8c",
//            "transactionIndex": "0x2f"
//        }]
//        """;
//    FilterLog[] ttt = JsonConvert.DeserializeObject<FilterLog[]>(logsss); 

//   var wl = pairContract.ContractBuilder.GetEventAbi("Swap").DecodeAllEventsDefaultTopics(ttt);
//    //EventABI.DecodeAllEventsDefaultTopics(logs);
//    foreach (var swapEventLog in swapEvents)
//    {
//        var ee = swapEventLog.Event.FirstOrDefault(x => x.Parameter.Name.Equals("amount0In"));
//        //var amount0In = (BigInteger)swapEventLog.Event.FirstOrDefault().Result .GetPropertyValue("amount0In");
//        //var amount1In = (BigInteger)swapEventLog.Event.GetPropertyValue("amount1In");
//        //var amount0Out = (BigInteger)swapEventLog.Event.GetPropertyValue("amount0Out");
//        //var amount1Out = (BigInteger)swapEventLog.Event.GetPropertyValue("amount1Out");

//        //// Fetch reserves to calculate price
//        var reservesFunction = pairContract.GetFunction("getReserves");
//        var reserves = await reservesFunction.CallDeserializingToObjectAsync<Reserves>();

//        //decimal price;
//        //if (amount0In > 0)
//        //{
//        //    // token0 is being bought with token1
//        //    price = (decimal)amount1In / (decimal)amount0In;
//        //}
//        //else
//        //{
//        //    // token0 is being sold for token1
//        //    price = (decimal)amount1Out / (decimal)amount0Out;
//        //}

//        //Console.WriteLine($"Block Number: {swapEventLog.Log.BlockNumber.Value}, Price: {price}");
//    }
//}

//public class Reserves
//{
//    [Parameter("uint112", "_reserve0", 1)]
//    public BigInteger Reserve0 { get; set; }

//    [Parameter("uint112", "_reserve1", 2)]
//    public BigInteger Reserve1 { get; set; }

//    [Parameter("uint32", "_blockTimestampLast", 3)]
//    public uint BlockTimestampLast { get; set; }
//}



