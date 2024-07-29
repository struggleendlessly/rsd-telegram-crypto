// See https://aka.ms/new-console-template for more information
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System.Text.Json;
using Shared.Telegram.Models;
using static System.Net.WebRequestMethods;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
//await Program1.Main();

var abi = """
[{"inputs":[],"payable":false,"stateMutability":"nonpayable","type":"constructor"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"owner","type":"address"},{"indexed":true,"internalType":"address","name":"spender","type":"address"},{"indexed":false,"internalType":"uint256","name":"value","type":"uint256"}],"name":"Approval","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"sender","type":"address"},{"indexed":false,"internalType":"uint256","name":"amount0","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount1","type":"uint256"},{"indexed":true,"internalType":"address","name":"to","type":"address"}],"name":"Burn","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"sender","type":"address"},{"indexed":false,"internalType":"uint256","name":"amount0","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount1","type":"uint256"}],"name":"Mint","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"sender","type":"address"},{"indexed":false,"internalType":"uint256","name":"amount0In","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount1In","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount0Out","type":"uint256"},{"indexed":false,"internalType":"uint256","name":"amount1Out","type":"uint256"},{"indexed":true,"internalType":"address","name":"to","type":"address"}],"name":"Swap","type":"event"},{"anonymous":false,"inputs":[{"indexed":false,"internalType":"uint112","name":"reserve0","type":"uint112"},{"indexed":false,"internalType":"uint112","name":"reserve1","type":"uint112"}],"name":"Sync","type":"event"},{"anonymous":false,"inputs":[{"indexed":true,"internalType":"address","name":"from","type":"address"},{"indexed":true,"internalType":"address","name":"to","type":"address"},{"indexed":false,"internalType":"uint256","name":"value","type":"uint256"}],"name":"Transfer","type":"event"},{"constant":true,"inputs":[],"name":"DOMAIN_SEPARATOR","outputs":[{"internalType":"bytes32","name":"","type":"bytes32"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"MINIMUM_LIQUIDITY","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"PERMIT_TYPEHASH","outputs":[{"internalType":"bytes32","name":"","type":"bytes32"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"internalType":"address","name":"","type":"address"},{"internalType":"address","name":"","type":"address"}],"name":"allowance","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"spender","type":"address"},{"internalType":"uint256","name":"value","type":"uint256"}],"name":"approve","outputs":[{"internalType":"bool","name":"","type":"bool"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[{"internalType":"address","name":"","type":"address"}],"name":"balanceOf","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"to","type":"address"}],"name":"burn","outputs":[{"internalType":"uint256","name":"amount0","type":"uint256"},{"internalType":"uint256","name":"amount1","type":"uint256"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"decimals","outputs":[{"internalType":"uint8","name":"","type":"uint8"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"factory","outputs":[{"internalType":"address","name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"getReserves","outputs":[{"internalType":"uint112","name":"_reserve0","type":"uint112"},{"internalType":"uint112","name":"_reserve1","type":"uint112"},{"internalType":"uint32","name":"_blockTimestampLast","type":"uint32"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"_token0","type":"address"},{"internalType":"address","name":"_token1","type":"address"}],"name":"initialize","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"kLast","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"to","type":"address"}],"name":"mint","outputs":[{"internalType":"uint256","name":"liquidity","type":"uint256"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"name","outputs":[{"internalType":"string","name":"","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[{"internalType":"address","name":"","type":"address"}],"name":"nonces","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"owner","type":"address"},{"internalType":"address","name":"spender","type":"address"},{"internalType":"uint256","name":"value","type":"uint256"},{"internalType":"uint256","name":"deadline","type":"uint256"},{"internalType":"uint8","name":"v","type":"uint8"},{"internalType":"bytes32","name":"r","type":"bytes32"},{"internalType":"bytes32","name":"s","type":"bytes32"}],"name":"permit","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"price0CumulativeLast","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"price1CumulativeLast","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"to","type":"address"}],"name":"skim","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"internalType":"uint256","name":"amount0Out","type":"uint256"},{"internalType":"uint256","name":"amount1Out","type":"uint256"},{"internalType":"address","name":"to","type":"address"},{"internalType":"bytes","name":"data","type":"bytes"}],"name":"swap","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"symbol","outputs":[{"internalType":"string","name":"","type":"string"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[],"name":"sync","outputs":[],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":true,"inputs":[],"name":"token0","outputs":[{"internalType":"address","name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"token1","outputs":[{"internalType":"address","name":"","type":"address"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":true,"inputs":[],"name":"totalSupply","outputs":[{"internalType":"uint256","name":"","type":"uint256"}],"payable":false,"stateMutability":"view","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"to","type":"address"},{"internalType":"uint256","name":"value","type":"uint256"}],"name":"transfer","outputs":[{"internalType":"bool","name":"","type":"bool"}],"payable":false,"stateMutability":"nonpayable","type":"function"},{"constant":false,"inputs":[{"internalType":"address","name":"from","type":"address"},{"internalType":"address","name":"to","type":"address"},{"internalType":"uint256","name":"value","type":"uint256"}],"name":"transferFrom","outputs":[{"internalType":"bool","name":"","type":"bool"}],"payable":false,"stateMutability":"nonpayable","type":"function"}]
""";

var web3 = new Web3("https://eth-mainnet.g.alchemy.com/v2/RwGrUoRsvsF9CPWEKUNOsCR_dkM6f4Uz");
var pairAddress = "0xYourPairContractAddress";


//{
//id: 1,
//jsonrpc: "2.0",
//method: "eth_getLogs",
//params:[
//{
//    fromBlock: "0x13773a4",
//toBlock: "0x137761c",
//address: [
//"0xc2eab7d33d3cb97692ecb231a5d0e4a649cb539d"
//],
//topics: [
//"0x1c411e9a96e071241c2f21f7726b17ae89e3cab4c78be50e062b03a9fffbbad1"
//]
//}
//]
//}

//await CalculateHistoricalPrices(web3, pairAddress, 0, BlockParameter.CreateLatest());

// static async Task CalculateHistoricalPrices(Web3 web3, string pairAddress, ulong fromBlock, BlockParameter toBlock)
//{
//    var pairContract = web3.Eth.GetContract(pairAbi, pairAddress);

//    // Fetch swap events
//    var swapEvent = pairContract.GetEvent("Swap");

//    var filter = swapEvent.CreateFilterInput(new BlockParameter(fromBlock), toBlock);
//    var swapEvents = await swapEvent.GetAllChangesDefault(filter);

//    foreach (var swapEventLog in swapEvents)
//    {
//        var amount0In = (BigInteger)swapEventLog.Event.GetPropertyValue("amount0In");
//        var amount1In = (BigInteger)swapEventLog.Event.GetPropertyValue("amount1In");
//        var amount0Out = (BigInteger)swapEventLog.Event.GetPropertyValue("amount0Out");
//        var amount1Out = (BigInteger)swapEventLog.Event.GetPropertyValue("amount1Out");

//        // Fetch reserves to calculate price
//        var reservesFunction = pairContract.GetFunction("getReserves");
//        var reserves = await reservesFunction.CallDeserializingToObjectAsync<Reserves>();

//        decimal price;
//        if (amount0In > 0)
//        {
//            // token0 is being bought with token1
//            price = (decimal)amount1In / (decimal)amount0In;
//        }
//        else
//        {
//            // token0 is being sold for token1
//            price = (decimal)amount1Out / (decimal)amount0Out;
//        }

//        Console.WriteLine($"Block Number: {swapEventLog.Log.BlockNumber.Value}, Price: {price}");
//    }
//}

var contract = web3.Eth.GetContract(abi, "0x9cF1190D71f7c5FCaF2033FfA4b017B1b0591EfD");

//var method = await contract.GetFunction("getReserves").CallAsync<BigInteger>();
//var getReservesFunction = contract.GetFunction("getReserves");
//var reserves = await getReservesFunction.CallDeserializingToObjectAsync<ReservesOutput>();

//Console.WriteLine($"Reserve 0: {reserves.Reserve0}");
//Console.WriteLine($"Reserve 1: {reserves.Reserve1}");
//Console.WriteLine($"Block Timestamp Last: {reserves.BlockTimestampLast}");


//var text ="{\r\n  \"?isRandom\": \"true\",\r\n  \"count\": \"3\"\r\n}"
//    "TEST MESSAGE!!!123 \n" +

//    "`0xbf769155ff776a717fb96616a567bb898b21bee6` \n" +
//    "❤️ \n" +
//    "DB: `11178` | `14163880` | `14190231` | 26351 \n" +

//    "[Owner](https://basescan.org/address/0xc0feb38ca691a7ccd508832571dc26b51de500e3) | [Token](https://basescan.org/token/0xbf769155ff776a717fb96616a567bb898b21bee6) | [DexScreener](https://dexscreener.com/base/0xbf769155ff776a717fb96616a567bb898b21bee6)";

//var ttt = "https://api.telegram.org/bot6721227973:AAHGbb1gjBn9CWh0zF9sOtVKA0g6iPp9KCE/sendMessage?message_thread_id=2268&chat_id=-1002144699173&text=1111&parse_mode=MarkDown&disable_web_page_preview=true";
//string urlString = $"https://api.telegram.org/bot6721227973:AAHGbb1gjBn9CWh0zF9sOtVKA0g6iPp9KCE/sendMessage?message_thread_id=136&chat_id=-1002144699173&text={text}&parse_mode=markdown&disable_web_page_preview=true";
//string deleteMessage = "https://api.telegram.org/bot6721227973:AAHGbb1gjBn9CWh0zF9sOtVKA0g6iPp9KCE/deleteMessage?message_thread_id=136&chat_id=-1002144699173&message_id=2802";
//using (var webclient = new WebClient())
//{
//    try
//    {
//        var response = await webclient.DownloadStringTaskAsync(urlString);
//        var ee = JsonSerializer.Deserialize<MessageSend>(response);
//    }
//    catch (Exception ee)
//    {

//        throw;
//    }

//}

////https://api.telegram.org/bot6721227973:AAHGbb1gjBn9CWh0zF9sOtVKA0g6iPp9KCE/deleteMessage?message_thread_id=136&chat_id=-1002144699173&message_id=2800
var syncEvent = web3.Eth.GetEvent<SyncEventDTO>("0x9cF1190D71f7c5FCaF2033FfA4b017B1b0591EfD");

// Specify the block range for yesterday (replace with actual block numbers)
var blockNumberYesterdayStart = new HexBigInteger(20411300);
var blockNumberYesterdayEnd = new HexBigInteger(20411932);

var filterAll = syncEvent.CreateFilterInput(new BlockParameter(blockNumberYesterdayStart), new BlockParameter(blockNumberYesterdayEnd));

var logs = await syncEvent.GetAllChangesAsync(filterAll);

foreach (var log in logs)
{
    Console.WriteLine($"Reserve0: {log.Event.Reserve0}, Reserve1: {log.Event.Reserve1}");
}


//var apiKey = "3ex6KSnDnxa98_q_F6CD26ByLMK4Gga-";
//var contractAddress = "0xc2eab7d33d3cb97692ecb231a5d0e4a649cb539d"; // Uniswap Pair contract address

//using (var client = new HttpClient())
//{
//    var response = await client.GetAsync($"https://eth-mainnet.alchemyapi.io/v2/{apiKey}/getAssetTransfers?fromBlock=0xC35000&toBlock=latest&contractAddresses[]={contractAddress}&category[]=external&category[]=token&category[]=internal");

//    if (response.IsSuccessStatusCode)
//    {
//        var content = await response.Content.ReadAsStringAsync();
//        Console.WriteLine(content);
//    }
//}


Console.WriteLine("Hello, World!");

[Event("Sync")]
public class SyncEventDTO : IEventDTO
{
    [Parameter("uint112", "_reserve0", 1, false)]
    public BigInteger Reserve0 { get; set; }

    [Parameter("uint112", "_reserve1", 2, false)]
    public BigInteger Reserve1 { get; set; }

}

[FunctionOutput]
public class ReservesOutput
{
    [Parameter("uint112", "_reserve0", 1)]
    public BigInteger Reserve0 { get; set; }

    [Parameter("uint112", "_reserve1", 2)]
    public BigInteger Reserve1 { get; set; }

    [Parameter("uint32", "_blockTimestampLast", 3)]
    public uint BlockTimestampLast { get; set; }
}



