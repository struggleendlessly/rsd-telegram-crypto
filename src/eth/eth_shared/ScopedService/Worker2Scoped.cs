using api_tokenSniffer;

using Data;
using Data.Models;

using etherscan;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public sealed class Worker2Scoped : IScopedProcessingService
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

        public Worker2Scoped(
            ILogger<Worker2Scoped> logger,
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

                _logger.LogInformation("Worker Worker2Scoped running at: {time}", DateTimeOffset.Now);

                try
                {
                    await Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker Worker2Scoped Exception: {message}", ex.Message);
                    _logger.LogError("Worker Worker2Scoped Exception: {stack}", ex.StackTrace);
                }

                var timeEndStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker Worker2Scoped running time: {time}", (timeEndStep1 - timeStartStep1).TotalSeconds);

                await Task.Delay(60000, stoppingToken);
            }
        }

        async Task Start()
        {
            //await getTokenSniffer.Start();
            //await getReservesLogs.Start();

            {
                _logger.LogInformation("Worker Worker2Scoped getSwapEvents running at: {time}", DateTimeOffset.Now);

                var _сount = await dbContext.EthSwapEvents.CountAsync();
                _logger.LogInformation("Worker Worker2Scoped getSwapEvents count before: {count}", _сount);

                var timeStart = DateTimeOffset.Now;
                /////////////////////
                await getSwapEvents.Start();
                /////////////////////
                var timeEnd = DateTimeOffset.Now;

                _сount = await dbContext.EthSwapEvents.CountAsync();
                _logger.LogInformation("Worker Worker2Scoped getSwapEvents count after: {count}", _сount);

                _logger.LogInformation("Worker Worker2Scoped getSwapEvents running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }

            {
                _logger.LogInformation("Worker Worker2Scoped getSwapEventsETHUSD running at: {time}", DateTimeOffset.Now);

                var _сount = await dbContext.EthSwapEventsETHUSD.CountAsync();
                _logger.LogInformation("Worker Worker2Scoped getSwapEventsETHUSD count before: {count}", _сount);

                var timeStart = DateTimeOffset.Now;
                /////////////////////
                await getSwapEventsETHUSD.Start();
                /////////////////////
                var timeEnd = DateTimeOffset.Now;

                _сount = await dbContext.EthSwapEventsETHUSD.CountAsync();
                _logger.LogInformation("Worker Worker2Scoped getSwapEventsETHUSD count after: {count}", _сount);

                _logger.LogInformation("Worker Worker2Scoped getSwapEventsETHUSD running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }
        }

        //async Task SendTlgrmMessageP0()
        //{
        //    var notDefault = default(DateTime).AddDays(1);
        //    var ethTrainData =
        //        dbContext.
        //        EthTrainData.
        //        //Where(x=>x.contractAddress == "0x3f4e95bf39bc676c4f7eaccd4d2d353fa2891190").
        //        //Where(
        //        //    x => x.walletCreated > notDefault &&
        //        //    x.BalanceOnCreating >= 0).
        //        //Take(2).
        //        Where(
        //            x => x.walletCreated > notDefault &&
        //            x.ABI != "no" &&
        //            x.BalanceOnCreating >= 0 &&
        //            x.tlgrmNewTokens == 0 &&
        //            x.isDead == false &&
        //            x.blockNumberInt > 20420936).
        //        ToList();

        //    var ids = ethTrainData.Select(x => x.blockNumberInt).ToList();
        //    var blocks = dbContext.EthBlock.Where(x => ids.Contains(x.numberInt)).ToList();

        //    var t = await tlgrmApi.SendPO(ethTrainData, blocks);

        //    foreach (var item in ethTrainData)
        //    {
        //        var resp = t.FirstOrDefault(x => x.contractAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase));

        //        if (resp is not null)
        //        {
        //            item.tlgrmNewTokens = resp.tlgrmMsgId;
        //        }
        //    }

        //    await dbContext.SaveChangesAsync();
        //}

    }
}
