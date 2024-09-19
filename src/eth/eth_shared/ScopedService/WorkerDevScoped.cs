using api_tokenSniffer;

using Cronos;

using Data;
using Data.Models;

using etherscan;

using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public sealed class WorkerDevScoped : IScopedProcessingService
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

        private const string schedule = "0/5 * * * *"; // every 5 min
        private readonly CronExpression _cron;
        public WorkerDevScoped(
            ILogger<WorkerDevScoped> logger,
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

            _cron = CronExpression.Parse(schedule);
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker WorkerDevScoped running");

                //var utcNow = DateTime.UtcNow;
                //var nextUtc = _cron.GetNextOccurrence(utcNow);
                //await Task.Delay(nextUtc.Value - utcNow, stoppingToken);

                var timeStartStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker WorkerDevScoped running at: {time}", DateTimeOffset.Now);

                try
                {                  
                    await Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker WorkerDevScoped Exception: {message}", ex.Message);
                    _logger.LogError("Worker WorkerDevScoped Exception: {stack}", ex.StackTrace);
                }

                var timeEndStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker WorkerDevScoped running time: {time}", (timeEndStep1 - timeStartStep1).TotalSeconds);

                
                //await Task.Delay(300_000, stoppingToken);
            }
        }

        async Task Start()
        {
            //await getTokenSniffer.Start();
            //await getReservesLogs.Start();

            {
                _logger.LogInformation("Worker WorkerDevScoped volumePrepare running at: {time}", DateTimeOffset.Now);

                //var _сount = await dbContext.EthSwapEvents.CountAsync();
                //_logger.LogInformation("Worker Worker4Scoped volumePrepare count before: {count}", _сount);

                var timeStart = DateTimeOffset.Now;
                /////////////////////

                _logger.LogInformation("Worker WorkerDevScoped volumePrepare .Start(5)");

                await volumePrepare.Start(5, 100);
                await volumeTracking.Start(5);
                /////////////////////
                var timeEnd = DateTimeOffset.Now;

                //_сount = await dbContext.EthSwapEvents.CountAsync();
                //_logger.LogInformation("Worker Worker2Scoped getSwapEvents count after: {count}", _сount);

                _logger.LogInformation("Worker WorkerDevScoped volumePrepare running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }
        }
    }
}
