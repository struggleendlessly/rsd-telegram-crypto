using Microsoft.Extensions.Options;

using Shared.ConfigurationOptions;
using Shared.Telegram;

namespace WorkerServiceReadTelegram
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Telegram telegram;
        private readonly OptionsTelegram optionsTelegram;

        public Worker(
            ILogger<Worker> logger, 
            Telegram telegram, 
            IOptions<OptionsTelegram> optionsTelegram)
        {
            _logger = logger;
            this.telegram = telegram;
            this.optionsTelegram = optionsTelegram.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await telegram.Start();
                await Task.Delay(optionsTelegram.api_delay_worker, stoppingToken);
            }
        }
    }
}
