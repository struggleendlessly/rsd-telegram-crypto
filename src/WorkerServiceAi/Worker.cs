using Microsoft.ML;

using ML.WorkerServiceAi;

using WorkerServiceAi.DB;
using WorkerServiceAi.ML;

namespace WorkerServiceAi
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DBContext dbContext;
        private readonly ClassificationBaseScan classification;
        private MLContext mlContext;
        public Worker(
            ILogger<Worker> logger,
            DBContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
            mlContext = new MLContext(seed: 0);
            this.classification = classification;
        }

        private static string _appPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        private static string _modelPath => Path.Combine(_appPath, "..", "..", "..", "Models", "modelBaseScanLearn2.zip");

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Training(dbContext).CompareTrainerEvaluations();

            var model = new MLModel_learn22(dbContext);
            model.Train(_modelPath);
            model.CreatePredictEngine(_modelPath);

            var dataSet =
                dbContext.
                ContractSourceCodeTrainDatas.
                Where(x=>x.Id == 20).
                FirstOrDefault();

            var input = new MLModel_learn22.ModelInput
            {
                didXXX = dataSet.didXXX,
                SourceCode = dataSet.SourceCode
            };

            var aa = model.Predict(input);
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    }
            //    await Task.Delay(1000, stoppingToken);
            //}
        }
    }
}