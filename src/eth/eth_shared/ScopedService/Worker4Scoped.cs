using api_tokenSniffer;

using Data;
using Data.Models;

using etherscan;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public sealed class Worker4Scoped : IScopedProcessingService
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
        private readonly VolumePrepare volumePrepare;
        private readonly VolumeTracking volumeTracking;
        private readonly GetSwapEventsETHUSD getSwapEventsETHUSD;
        private readonly GetBalanceOnCreating getBalanceOnCreating;

        public Worker4Scoped(
            ILogger<Worker4Scoped> logger,
            IsDead isDead,
            GetPair getPair,
            dbContext dbContext,
            EtherscanApi etherscanApi,
            GetWalletAge getWalletAge,
            //tlgrmApi.tlgrmApi tlgrmApi,
            GetSourceCode getSourceCode,
            GetSwapEvents getSwapEvents,
            GetTokenSniffer getTokenSniffer,
            GetReservesLogs getReservesLogs,
            VolumePrepare volumePrepare,
            VolumeTracking volumeTracking,
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
            this.volumePrepare = volumePrepare;
            this.volumeTracking = volumeTracking;
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

                _logger.LogInformation("Worker Worker4Scoped running at: {time}", DateTimeOffset.Now);

                try
                {
                    await Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker Worker4Scoped Exception: {message}", ex.Message);
                    _logger.LogError("Worker Worker4Scoped Exception: {stack}", ex.StackTrace);
                }

                var timeEndStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker Worker4Scoped running time: {time}", (timeEndStep1 - timeStartStep1).TotalSeconds);

                await Task.Delay(300, stoppingToken);
            }
        }

        async Task Start()
        {
            //await getTokenSniffer.Start();
            //await getReservesLogs.Start();

            {
                _logger.LogInformation("Worker Worker4Scoped volumePrepare running at: {time}", DateTimeOffset.Now);

                var _сount = await dbContext.EthSwapEvents.CountAsync();
                _logger.LogInformation("Worker Worker4Scoped volumePrepare count before: {count}", _сount);

                var timeStart = DateTimeOffset.Now;
                /////////////////////
                //await volumePrepare.Start(1);
                //await volumeTracking.Start(60);
                await volumePrepare.Start(5);
                await volumePrepare.Start(30);
                await volumePrepare.Start(60);
                /////////////////////
                var timeEnd = DateTimeOffset.Now;

                //_сount = await dbContext.EthSwapEvents.CountAsync();
                //_logger.LogInformation("Worker Worker2Scoped getSwapEvents count after: {count}", _сount);

                _logger.LogInformation("Worker Worker4Scoped volumePrepare running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }

            //{
            //    _logger.LogInformation("Worker Worker2Scoped getSwapEventsETHUSD running at: {time}", DateTimeOffset.Now);

            //    var _сount = await dbContext.EthSwapEventsETHUSD.CountAsync();
            //    _logger.LogInformation("Worker Worker2Scoped getSwapEventsETHUSD count before: {count}", _сount);

            //    var timeStart = DateTimeOffset.Now;
            //    /////////////////////
            //    //await getSwapEventsETHUSD.Start();
            //    /////////////////////
            //    var timeEnd = DateTimeOffset.Now;

            //    _сount = await dbContext.EthSwapEventsETHUSD.CountAsync();
            //    _logger.LogInformation("Worker Worker2Scoped getSwapEventsETHUSD count after: {count}", _сount);

            //    _logger.LogInformation("Worker Worker2Scoped getSwapEventsETHUSD running time: {time}", (timeEnd - timeStart).TotalSeconds);
            //}
        }
    }
}
