﻿using api_tokenSniffer;

using Data;
using Data.Models;

using etherscan;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.ComponentModel.DataAnnotations;

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
            ILogger<Worker1Scoped> logger,
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

                _logger.LogInformation("Worker Worker1Scoped running at: {time}", DateTimeOffset.Now);

                try
                {
                    await Start();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Worker Worker1Scoped Exception: {message}", ex.Message);
                    _logger.LogError("Worker Worker1Scoped Exception: {stack}", ex.StackTrace);
                }

                var timeEndStep1 = DateTimeOffset.Now;

                _logger.LogInformation("Worker Worker1Scoped running time: {time}", (timeEndStep1 - timeStartStep1).TotalSeconds);

                await Task.Delay(20000, stoppingToken);
            }
        }

        async Task Start()
        {
            {
                _logger.LogInformation("Worker Worker1Scoped isDead running at: {time}", DateTimeOffset.Now);

                var _сount = await dbContext.EthTrainData.Where(x => x.isDead == true).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped isDead count before: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.isDead == false).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped isNotDead count before: {count}", _сount);

                var timeStart = DateTimeOffset.Now;
                /////////////////////
                await isDead.Start();
                /////////////////////
                var timeEnd = DateTimeOffset.Now;

                _сount = await dbContext.EthTrainData.Where(x => x.isDead == true).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped isDead count after: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.isDead == false).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped isNotDead count after: {count}", _сount);

                _logger.LogInformation("Worker Worker1Scoped isDead running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }

            {
                _logger.LogInformation("Worker Worker1Scoped getBalanceOnCreating running at: {time}", DateTimeOffset.Now);

                var _сount = await dbContext.EthTrainData.Where(x => x.BalanceOnCreating >= 0).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getBalanceOnCreating count positive before: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.BalanceOnCreating < 0).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getBalanceOnCreating count negative before: {count}", _сount);

                var timeStart = DateTimeOffset.Now;
                /////////////////////
                await getBalanceOnCreating.Start();
                /////////////////////
                var timeEnd = DateTimeOffset.Now;

                _сount = await dbContext.EthTrainData.Where(x => x.BalanceOnCreating >= 0).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getBalanceOnCreating сount positive after: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.BalanceOnCreating < 0).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getBalanceOnCreating count negative after: {count}", _сount);

                _logger.LogInformation("Worker Worker1Scoped getBalanceOnCreating running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }

            {
                _logger.LogInformation("Worker Worker1Scoped getSourceCode running at: {time}", DateTimeOffset.Now);

                var _сount = await dbContext.EthTrainData.Where(x => x.ABI == "no").CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getSourceCode '== no' count before: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.ABI == null).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getSourceCode '== null' count before: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.ABI != "no" && x.ABI != null).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getSourceCode '!= no && != null' count before: {count}", _сount);

                var timeStart = DateTimeOffset.Now;
                /////////////////////
                await getSourceCode.Start();
                /////////////////////
                var timeEnd = DateTimeOffset.Now;

                _сount = await dbContext.EthTrainData.Where(x => x.ABI == "no").CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getSourceCode '== no' сount after: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.ABI == null).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getSourceCode '== null' count after: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.ABI != "no" && x.ABI != null).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getSourceCode '!= no && != null' count after: {count}", _сount);

                _logger.LogInformation("Worker Worker1Scoped getSourceCode running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }

            {
                _logger.LogInformation("Worker Worker1Scoped getWalletAge running at: {time}", DateTimeOffset.Now);

                var _сount = await dbContext.EthTrainData.Where(x => x.walletCreated == default).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getWalletAge default count before: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.walletCreated != default).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getWalletAge not default count before: {count}", _сount);

                var timeStart = DateTimeOffset.Now;
                /////////////////////
                await getWalletAge.Start();
                /////////////////////
                var timeEnd = DateTimeOffset.Now;

                _сount = await dbContext.EthTrainData.Where(x => x.walletCreated == default).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getWalletAge default count after: {count}", _сount);

                _сount = await dbContext.EthTrainData.Where(x => x.walletCreated != default).CountAsync();
                _logger.LogInformation("Worker Worker1Scoped getWalletAge not default count after: {count}", _сount);

                _logger.LogInformation("Worker Worker1Scoped getWalletAge running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }

            {
                var timeStart = DateTimeOffset.Now;
                _logger.LogInformation("Worker Worker1Scoped SendTlgrmMessageP0 running at: {time}", DateTimeOffset.Now);
                /////////////////////
                await SendTlgrmMessageP0();
                /////////////////////
                var timeEnd = DateTimeOffset.Now;
                _logger.LogInformation("Worker Worker1Scoped SendTlgrmMessageP0 running time: {time}", (timeEnd - timeStart).TotalSeconds);
            }
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

                if (t1 is not null)
                {
                    item.EthSwapEvents = new List<EthSwapEvents>(t1);
                }
            }

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

            var t_public = await tlgrmApi.SendPO(ethTrainData, blocks, "public");
            var t_closed = await tlgrmApi.SendPO(ethTrainData, blocks, "closed");
        }
    }
}
