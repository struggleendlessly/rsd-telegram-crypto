using api_alchemy.Eth;
using api_alchemy.Eth.ResponseDTO;

using Data;
using Data.Models;

using eth_shared.Map;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using nethereum;

using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.Util;

using System.Globalization;

namespace eth_shared
{
    public class GetSwapEvents
    {
        private readonly ILogger logger;
        private readonly ApiWeb3 ApiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;

        int lastEthBlockNumber = 0;
        int lastProcessedBlock = 18911035;
        int lastBlockToProcess = 0;
        private Dictionary<string, Token0AndToken1> token0AndToken1Cache = new();

        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        string decimalCeparator = ".";

        public GetSwapEvents(
            ILogger<GetSwapEvents> logger,
            ApiWeb3 ApiWeb3,
            EthApi apiAlchemy,
            dbContext dbContext
            )
        {
            this.logger = logger;
            this.ApiWeb3 = ApiWeb3;
            this.dbContext = dbContext;
            this.apiAlchemy = apiAlchemy;
        }

        public async Task Start()
        {
            lastEthBlockNumber = (await dbContext.EthBlock.OrderByDescending(x => x.numberInt).Take(1).SingleAsync()).numberInt;

            if (await dbContext.EthSwapEvents.AnyAsync())
            {
                lastProcessedBlock = await dbContext.EthSwapEvents.MaxAsync(x => x.blockNumberInt) + 1;
            }

            lastBlockToProcess = lastProcessedBlock + 2000;

            if (lastBlockToProcess > lastEthBlockNumber)
            {
                lastBlockToProcess = lastEthBlockNumber;
            }

            var tokensToProcess = await GetTokensToProcess();
            var tokensToProcessBactch = tokensToProcess.Batch(1000);

            foreach (var item in tokensToProcessBactch)
            {
                var unfiltered = await Get(item.ToList());
                var validated = Validate(unfiltered);

                if (validated.Count == 0)
                {
                    var t = new EthSwapEvents();
                    t.blockNumberInt = lastBlockToProcess;

                    dbContext.EthSwapEvents.Add(t);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    var decoded = DecodeSwapEvents(validated);
                    var addressesToProcess =
                        decoded.
                        Select(x => x.Log.Address).
                        Distinct().
                        Select((v, i) => (v, i)).
                        ToList();

                    var token0 = await GetToken0(addressesToProcess);
                    Thread.Sleep(1000);
                    var token1 = await GetToken1(addressesToProcess);
                    GetToken0AndToken1(addressesToProcess, token0, token1);
                    var processed = await ProcessDecoded(decoded, tokensToProcess);
                    var saved = await SaveToDB_update(processed);
                }
            }
        }

        public async Task<List<getTotalSupplyDTO>> GetToken0(List<(string, int)> events)
        {
            List<getTotalSupplyDTO> res = new();

            var diff = events.Count();
            var items = events;

            Func<List<(string, int)>, int, Task<List<getTotalSupplyDTO>>> apiMethod = apiAlchemy.eth_callToken0;

            res = await apiAlchemy.executeBatchCall(events, apiMethod, diff);

            return res;
        }

        public async Task<List<getTotalSupplyDTO>> GetToken1(List<(string, int)> events)
        {
            List<getTotalSupplyDTO> res = new();

            var diff = events.Count();
            var items = events;

            Func<List<(string, int)>, int, Task<List<getTotalSupplyDTO>>> apiMethod = apiAlchemy.eth_callToken1;

            res = await apiAlchemy.executeBatchCall(events, apiMethod, diff);

            return res;
        }

        private async Task<int> SaveToDB_update
            (List<EthSwapEvents> ethSwapEvents)
        {
            var res = 0;

            dbContext.EthSwapEvents.AddRange(ethSwapEvents);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        private void GetToken0AndToken1(
            List<(string v, int i)> addressesToProcess,
            List<getTotalSupplyDTO> token0,
            List<getTotalSupplyDTO> token1
            )
        {
            foreach (var item in addressesToProcess)
            {
                Token0AndToken1 res = new();

                var t0 = token0.Where(x => x.id == item.i).FirstOrDefault();
                var t1 = token1.Where(x => x.id == item.i).FirstOrDefault();

                res.token0 = t0.result.Replace("0x", "").TrimStart('0');
                res.token0 = "0x" + res.token0;

                res.token1 = t1.result.Replace("0x", "").TrimStart('0');
                res.token1 = "0x" + res.token1;

                token0AndToken1Cache.TryAdd(item.v, res);
            }
        }

        public async Task<List<EthSwapEvents>> ProcessDecoded(
            List<EventLog<List<ParameterOutput>>> decoded,
            List<EthTrainData> ethTrainDatas
            )
        {
            List<EthSwapEvents> res = new();

            foreach (var item in decoded)
            {
                var logs = item.Log;
                var events = item.Event;
                var ethTrainData = ethTrainDatas.Where(x => x.pairAddress.Equals(logs.Address, StringComparison.InvariantCultureIgnoreCase)).Single();

                var tokens01 = token0AndToken1Cache[logs.Address];

                var ethSwapEvents = events.Map(ethTrainData, decimalCeparator, tokens01);

                ethSwapEvents.pairAddress = logs.Address;
                ethSwapEvents.blockNumberInt = Convert.ToInt32(logs.BlockNumber.ToString());

                BigDecimal EthIn = BigDecimal.Parse(ethSwapEvents.EthIn);
                BigDecimal EthOut = BigDecimal.Parse(ethSwapEvents.EthOut);
                BigDecimal TokenIn = BigDecimal.Parse(ethSwapEvents.TokenIn);
                BigDecimal TokenOut = BigDecimal.Parse(ethSwapEvents.TokenOut);

                BigDecimal price = 0.0;

                if (EthIn > 0 &&
                    TokenOut > 0)
                {
                    // token0 is being bought with token1
                    price = EthIn / TokenOut;
                    ethSwapEvents.isBuy = true;
                }

                if (EthOut > 0 &&
                    TokenIn > 0)

                {
                    // token0 is being sold for token1
                    price = EthOut / TokenIn;
                }

                ethSwapEvents.txsHash = logs.TransactionHash;
                ethSwapEvents.priceEth = (double)price;
                ethSwapEvents.EthTrainData = ethTrainData;

                res.Add(ethSwapEvents);
            }

            return res;
        }

        public List<EventLog<List<ParameterOutput>>> DecodeSwapEvents(List<getSwapDTO> validated)
        {
            var logs = validated.SelectMany(x => x.result);
            var logsJson = System.Text.Json.JsonSerializer.Serialize(logs);
            var res = ApiWeb3.DecodeSwapEvents(logsJson);

            return res;
        }

        public List<getSwapDTO> Validate(List<getSwapDTO> collection)
        {
            List<getSwapDTO> res = new();

            foreach (var item in collection)
            {
                if (item.result is not null &&
                    item.result.Count() > 0)
                {
                    res.Add(item);
                }
            }

            return res;
        }
        public async Task<List<EthTrainData>> GetTokensToProcess()
        {
            var res = await
                dbContext.
                EthTrainData.
                Where(
                    x =>
                    x.pairAddress != "no" &&
                    (x.DeadBlockNumber > lastBlockToProcess || x.DeadBlockNumber == 0) &&
                    x.blockNumberInt < lastBlockToProcess
                    ).
                OrderBy(x => x.blockNumberInt).
                //Where(x => x.pairAddress == "0xb1b665fb26d29934a678e79de4d95edb0bf2c33e").
                ToListAsync();

            return res;
        }

        public async Task<List<getSwapDTO>> Get(List<EthTrainData> ethTrainDatas)
        {
            List<getSwapDTO> res = new();

            var diff = ethTrainDatas.Count();
            var items = ethTrainDatas;

            List<(string, string, string)> t = new();

            foreach (var item in ethTrainDatas)
            {
                t.Add((item.pairAddress, "0x" + lastProcessedBlock.ToString("x"), "0x" + lastBlockToProcess.ToString("x")));
            }

            Func<List<(string, string, string)>, int, Task<List<getSwapDTO>>> apiMethod = apiAlchemy.getSwapLogs;

            res = await apiAlchemy.executeBatchCall(t, apiMethod, diff);

            return res;
        }

        public class Token0AndToken1
        {
            public string token0 { get; set; }
            public string token1 { get; set; }
        }
    }
}
