using api_tokenSniffer;

using Data;
using Data.Models;

using etherscan;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public sealed class Worker3Scoped : IScopedProcessingService
    {
        private List<EthTrainData> ethTrainDatas = new();

        private readonly ILogger _logger;
        private readonly IsDead isDead;
        private readonly GetPair getPair;
        private readonly dbContext dbContext;
        private readonly tlgrmApi.tlgrmApi tlgrmApi;
        private readonly EtherscanApi etherscanApi;
        private readonly GetWalletAge getWalletAge;
        private readonly GetSourceCode getSourceCode;
        private readonly GetSwapEvents getSwapEvents;
        private readonly GetTokenSniffer getTokenSniffer;
        private readonly GetReservesLogs getReservesLogs;
        private readonly GetSwapEventsETHUSD getSwapEventsETHUSD;
        private readonly GetBalanceOnCreating getBalanceOnCreating;

        public Worker3Scoped(
            ILogger<Worker3Scoped> logger,
            IsDead isDead,
            GetPair getPair,
            dbContext dbContext,
            EtherscanApi etherscanApi,
            GetWalletAge getWalletAge,
            tlgrmApi.tlgrmApi tlgrmApi,
            GetSourceCode getSourceCode,
            GetSwapEvents getSwapEvents,
            GetTokenSniffer getTokenSniffer,
            GetReservesLogs getReservesLogs,
            GetSwapEventsETHUSD getSwapEventsETHUSD,
            GetBalanceOnCreating getBalanceOnCreating
            )
        {
            this._logger = logger;
            this.isDead = isDead;
            this.getPair = getPair;
            this.tlgrmApi = tlgrmApi;
            this.dbContext = dbContext;
            this.getWalletAge = getWalletAge;
            this.etherscanApi = etherscanApi;
            this.getSourceCode = getSourceCode;
            this.getSwapEvents = getSwapEvents;
            this.getReservesLogs = getReservesLogs;
            this.getTokenSniffer = getTokenSniffer;
            this.getSwapEventsETHUSD = getSwapEventsETHUSD;
            this.getBalanceOnCreating = getBalanceOnCreating;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var timeStartStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker Worker3Scoped running at: {time}", DateTimeOffset.Now);

                try
                {
                    await Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker Worker3Scoped Exception: {message}", ex.Message);
                    _logger.LogError("Worker Worker3Scoped Exception: {stack}", ex.StackTrace);
                }

                var timeEndStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker Worker3Scoped running time: {time}", (timeEndStep1 - timeStartStep1).TotalSeconds);

                await Task.Delay(300000, stoppingToken);
            }
        }

        async Task Start()
        {
            {
                _logger.LogInformation("Worker Worker3Scoped getPair running at: {time}", DateTimeOffset.Now);

                var _сount = await dbContext.EthTrainData.Where(x => x.pairAddress == "").CountAsync();
                _logger.LogInformation("Worker Worker3Scoped getPair  == '' count before: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.pairAddress != "").CountAsync();
                _logger.LogInformation("Worker Worker3Scoped getPair  != '' count before: {count}", _сount);

                var timeStart = DateTimeOffset.Now;
                /////////////////////
                await getPair.Start();
                /////////////////////
                var timeEnd = DateTimeOffset.Now;

                _сount = await dbContext.EthTrainData.Where(x => x.pairAddress == "").CountAsync();
                _logger.LogInformation("Worker Worker3Scoped getPair == '' count after: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.pairAddress != "").CountAsync();
                _logger.LogInformation("Worker Worker3Scoped getPair != '' count after: {count}", _сount);

                _logger.LogInformation("Worker Worker3Scoped getPair running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }

            {
                var timeStart = DateTimeOffset.Now;
                _logger.LogInformation("Worker Worker3Scoped SendTlgrmMessageP10 running at: {time}", DateTimeOffset.Now);
                /////////////////////
                await SendTlgrmMessageP10();
                /////////////////////
                var timeEnd = DateTimeOffset.Now;
                _logger.LogInformation("Worker Worker3Scoped SendTlgrmMessageP10 running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }
        }

        public async Task SendTlgrmMessageP10()
        {
            var notDefault = default(DateTime).AddDays(1);
            var ethTrainData =
                dbContext.
                EthTrainData.
                //Where(
                //    x => x.walletCreated > notDefault &&
                //    x.BalanceOnCreating >= 0).
                //Take(2).
                Where(
                    x =>
                    x.pairAddress != "" &&
                    x.pairAddress != "no" &&
                    x.tlgrmLivePairs == 0 &&
                    x.BalanceOnCreating >= 0 &&
                    x.isDead == false &&
                    x.blockNumberInt > 20456589).
                ToList();

            IEnumerable<int> EthTrainDataIds = ethTrainData.Select(v => v.Id);

            var swaps =
                await
                dbContext.
                EthSwapEvents.
                Where(x => EthTrainDataIds.Contains((int)x.EthTrainDataId)).
                GroupBy(x => x.EthTrainDataId).
                Select(g => g.OrderByDescending(row => row.Id).Take(1)).
                ToListAsync();

            foreach (var item in ethTrainData)
            {
                var t1 =
                    swaps.
                    Where(x => x.Any(v => v.EthTrainDataId == item.Id)).
                    Select(x => x).
                    FirstOrDefault();

                item.EthSwapEvents = new List<EthSwapEvents>(t1);
            }

            var ids = ethTrainData.Select(x => x.blockNumberInt).ToList();
            var blocks = dbContext.EthBlock.Where(x => ids.Contains(x.numberInt)).ToList();

            var t = await tlgrmApi.SendP1O(ethTrainData, blocks);

            foreach (var item in ethTrainData)
            {
                var resp = t.FirstOrDefault(x => x.contractAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase));

                if (resp is not null)
                {
                    item.tlgrmLivePairs = resp.tlgrmMsgId;
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
