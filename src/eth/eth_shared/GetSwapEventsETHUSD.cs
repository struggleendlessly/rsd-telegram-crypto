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
    public class GetSwapEventsETHUSD
    {
        private readonly ILogger logger;
        private readonly ApiWeb3 ApiWeb3;
        private readonly EthApi apiAlchemy;
        private readonly dbContext dbContext;

        int lastEthBlockNumber = 0;
        int lastProcessedBlock = 12376729;
        int lastBlockToProcess = 0;

        string contractAddress = "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2";
        string pairAddressETHUSD = "0xa478c2975ab1ea89e8196811f51a7b7ade33eb11";

        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        string decimalCeparator = ".";

        public GetSwapEventsETHUSD(
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

            if (await dbContext.EthSwapEventsETHUSD.AnyAsync())
            {
                lastProcessedBlock = await dbContext.EthSwapEventsETHUSD.MaxAsync(x => x.blockNumberInt) + 1;
            }

            lastBlockToProcess = lastProcessedBlock + 2000;

            if (lastBlockToProcess > lastEthBlockNumber)
            {
                lastBlockToProcess = lastEthBlockNumber;
            }

            var unfiltered = await Get();
            var validated = Validate(unfiltered);

            if (validated.Count == 0)
            {
                var t = new EthSwapEventsETHUSD();
                t.blockNumberInt = lastBlockToProcess;

                dbContext.EthSwapEventsETHUSD.Add(t);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                var decoded = DecodeSwapEvents(validated);
                var processed = ProcessDecoded(decoded);
                var saved = await SaveToDB_update(processed);
            }
        }

        private async Task<int> SaveToDB_update
            (List<EthSwapEventsETHUSD> ethSwapEvents)
        {
            var res = 0;

            dbContext.EthSwapEventsETHUSD.AddRange(ethSwapEvents);
            res = await dbContext.SaveChangesAsync();

            return res;
        }

        public List<EthSwapEventsETHUSD> ProcessDecoded(
            List<EventLog<List<ParameterOutput>>> decoded
            )
        {
            List<EthSwapEventsETHUSD> res = new();

            foreach (var item in decoded)
            {
                var logs = item.Log;
                var events = item.Event;

                var ethSwapEvents = events.Map(
                    contractAddress,
                    18,
                    decimalCeparator);

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
                    price = TokenOut / EthIn;
                    ethSwapEvents.isBuyDai = true;
                }

                if (EthOut > 0 &&
                    TokenIn > 0)

                {
                    ethSwapEvents.isBuyEth = true;
                    price = TokenIn / EthOut;
                }

                ethSwapEvents.txsHash = logs.TransactionHash;
                ethSwapEvents.priceEthInUsd = (double)price;

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

        public async Task<List<getSwapDTO>> Get()
        {
            List<getSwapDTO> res = new();

            List<(string, string, string)> t = new();

            if (lastProcessedBlock <= lastBlockToProcess)
            {
                t.Add((pairAddressETHUSD, "0x" + lastProcessedBlock.ToString("x"), "0x" + lastBlockToProcess.ToString("x")));

                Func<List<(string, string, string)>, int, Task<List<getSwapDTO>>> apiMethod = apiAlchemy.getSwapLogs;

                res = await apiAlchemy.executeBatchCall(t, apiMethod, 1);
            }

            return res;
        }
    }
}
