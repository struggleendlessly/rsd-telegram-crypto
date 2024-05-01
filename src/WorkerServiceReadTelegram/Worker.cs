using Shared.Telegram;

namespace WorkerServiceReadTelegram
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly Telegram telegram;

        public Worker(ILogger<Worker> logger, Telegram telegram)
        {
            _logger = logger;
            this.telegram = telegram;
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
                await Task.Delay(100000, stoppingToken);
            }
        }
    }
}
