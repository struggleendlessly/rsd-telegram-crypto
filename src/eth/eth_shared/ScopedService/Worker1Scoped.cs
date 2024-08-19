using api_tokenSniffer;

using Data;
using Data.Models;

using etherscan;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eth_shared
{
    public sealed class Worker1Scoped : IScopedProcessingService
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

        public Worker1Scoped(
            ILogger<Step2> logger,
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

                _logger.LogInformation("Worker step3 running at: {time}", DateTimeOffset.Now);

                try
                {
                    await Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker step3 Exception: {message}", ex.Message);
                    _logger.LogError("Worker step3 Exception: {stack}", ex.StackTrace);
                }

                var timeEndStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker step3 processed blocks: {block}", await dbContext.EthBlock.CountAsync());
                _logger.LogInformation("Worker step3 running time: {time}", (timeEndStep1 - timeStartStep1).TotalSeconds);

                await Task.Delay(60000, stoppingToken);
            }
        }

        async Task Start()
        {
            await isDead.Start();
            await getBalanceOnCreating.Start();
            await getSourceCode.Start();
            await getWalletAge.Start();

            await SendTlgrmMessageP0();
        }

        async Task SendTlgrmMessageP0()
        {
            var notDefault = default(DateTime).AddDays(1);
            var ethTrainData =
                dbContext.
                EthTrainData.
                //Where(x=>x.contractAddress == "0x3f4e95bf39bc676c4f7eaccd4d2d353fa2891190").
                //Where(
                //    x => x.walletCreated > notDefault &&
                //    x.BalanceOnCreating >= 0).
                //Take(2).
                Where(
                    x => x.walletCreated > notDefault &&
                    x.ABI != "no" &&
                    x.BalanceOnCreating >= 0 &&
                    x.tlgrmNewTokens == 0 &&
                    x.isDead == false &&
                    x.blockNumberInt > 20420936).
                ToList();

            var ids = ethTrainData.Select(x => x.blockNumberInt).ToList();
            var blocks = dbContext.EthBlock.Where(x => ids.Contains(x.numberInt)).ToList();

            var t = await tlgrmApi.SendPO(ethTrainData, blocks);

            foreach (var item in ethTrainData)
            {
                var resp = t.FirstOrDefault(x => x.contractAddress.Equals(item.contractAddress, StringComparison.InvariantCultureIgnoreCase));

                if (resp is not null)
                {
                    item.tlgrmNewTokens = resp.tlgrmMsgId;
                }
            }

            await dbContext.SaveChangesAsync();
        }

    }
}
