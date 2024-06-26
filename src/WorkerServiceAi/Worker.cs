using Microsoft.ML;
using Microsoft.ML.Data;

using ML.learng2.WorkerServiceAi;
using ML.WorkerServiceAi;

using System.Data;

using WorkerServiceAi.DB;

using static Microsoft.ML.DataOperationsCatalog;

namespace WorkerServiceAi
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly DBContext dbContext;
        private readonly ClassificationBaseScan classification;
        private MLContext mlContext;
        public Worker(ILogger<Worker> logger,
            DBContext dbContext,
            ClassificationBaseScan classification)
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
            MLModel_learn2.Train(_modelPath);
            MLModel_learn22.Train(_modelPath);

            var ee = ";";
            classification.Start();
            //var a = new Classification();
            //var dataSet = dbContext.Learn22.ToList();


            //IDataView data = mlContext.Data.LoadFromEnumerable(dataSet);

            //TrainTestData dataSplit = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            //IDataView trainData = dataSplit.TrainSet;
            //IDataView testData = dataSplit.TestSet;

            //var pipeline = ProcessData();

            //var trainingPipeline = BuildAndTrainModel(pipeline);

            //var trainedModel = trainingPipeline.Fit(trainData);
            //var predEngine = mlContext.Model.CreatePredictionEngine<Learn22, IssuePrediction>(trainedModel);

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    }
            //    await Task.Delay(1000, stoppingToken);
            //}
        }

        public class IssuePrediction
        {
            [ColumnName("PredictedLabel")]
            public string? isGood;
        }

        IEstimator<ITransformer> ProcessData()
        {
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "isGood", outputColumnName: "Label")
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "contractCode", outputColumnName: "contractCodeFeaturized"))
                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "contractByteCode", outputColumnName: "contractByteCodeFeaturized"))
                .Append(mlContext.Transforms.Concatenate("Features", "contractCodeFeaturized", "contractByteCodeFeaturized"));

            return pipeline;
        }

        IEstimator<ITransformer> BuildAndTrainModel(IEstimator<ITransformer> pipeline)
        {
            var trainingPipeline = pipeline.Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
               .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            return trainingPipeline;
        }
    }
}