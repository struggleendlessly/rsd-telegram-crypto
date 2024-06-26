using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;

using WorkerServiceAi.DB;

using static Microsoft.ML.DataOperationsCatalog;

namespace WorkerServiceAi
{
    public class IssuePredictionBaseScan
    {
        [ColumnName("PredictedLabel")]
        public string isGood;
    }
    public class ClassificationBaseScan
    {

        // <SnippetDeclareGlobalVariables>
        private static string _appPath => Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
        private static string _modelPath => Path.Combine(_appPath, "..", "..", "..", "Models", "modelBaseScan.zip");
        IDataView _testData;
        private readonly DBContext dbContext;
        private static MLContext _mlContext;
        private static PredictionEngine<Learn22Base, IssuePredictionBaseScan> _predEngine;
        private static ITransformer _trainedModel;
        static IDataView _trainingDataView;
        // </SnippetDeclareGlobalVariables>
        public ClassificationBaseScan(DBContext dbContext)
        {
            this.dbContext = dbContext;
            // Create MLContext to be shared across the model creation workflow objects
            // Set a random seed for repeatable/deterministic results across multiple trainings.
            // <SnippetCreateMLContext>
            _mlContext = new MLContext(seed: 0);
            // </SnippetCreateMLContext>

            // STEP 1: Common data loading configuration
            // CreateTextReader<GitHubIssue>(hasHeader: true) - Creates a TextLoader by inferencing the dataset schema from the GitHubIssue data model type.
            // .Read(_trainDataPath) - Loads the training text file into an IDataView (_trainingDataView) and maps from input columns to IDataView columns.
            Console.WriteLine($"=============== Loading Dataset  ===============");

            var dataSet = dbContext.Learn22.ToList();
            _trainingDataView = _mlContext.Data.LoadFromEnumerable(dataSet);

            var dataSetTest = dbContext.Learn22_testData.ToList();
            _testData = _mlContext.Data.LoadFromEnumerable(dataSet);
        }
        public void Start()
        {

            Console.WriteLine($"=============== Finished Loading Dataset  ===============");

            // <SnippetSplitData>
            //   var (trainData, testData) = _mlContext.MulticlassClassification.TrainTestSplit(_trainingDataView, testFraction: 0.1);
            // </SnippetSplitData>

            // <SnippetCallProcessData>
            var pipeline = ProcessData();
            // </SnippetCallProcessData>

            // <SnippetCallBuildAndTrainModel>
            var trainingPipeline = BuildAndTrainModel(_trainingDataView, pipeline);
            // </SnippetCallBuildAndTrainModel>

            // <SnippetCallEvaluate>
            Evaluate(_trainingDataView.Schema);
            // </SnippetCallEvaluate>

            // <SnippetCallPredictIssue>
            PredictIssue();
            // </SnippetCallPredictIssue>    
        }
        public static IEstimator<ITransformer> ProcessData()
        {
            Console.WriteLine($"=============== Processing Data ===============");
            // STEP 2: Common data process configuration with pipeline data transformations
            // <SnippetMapValueToKey>
            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "isGood", outputColumnName: "Label")
                // </SnippetMapValueToKey>
                // <SnippetFeaturizeText>
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "contractCode", outputColumnName: "contractCodeFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "contractByteCode", outputColumnName: "contractByteCodeFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "contractABI", outputColumnName: "contractABIFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "totalSupply", outputColumnName: "totalSupplyFeaturized"))
                .Append(_mlContext.Transforms.Text.FeaturizeText(inputColumnName: "divisor", outputColumnName: "divisorFeaturized"))
                            // </SnippetFeaturizeText>
                            // <SnippetConcatenate>
                            .Append(_mlContext.Transforms.Concatenate("Features", "contractCodeFeaturized", "contractByteCodeFeaturized", "contractABIFeaturized", "totalSupplyFeaturized", "divisorFeaturized"));
            // </SnippetConcatenate>
            //Sample Caching the DataView so estimators iterating over the data multiple times, instead of always reading from file, using the cache might get better performance.
            // <SnippetAppendCache>
            //.AppendCacheCheckpoint(_mlContext);
            // </SnippetAppendCache>

            Console.WriteLine($"=============== Finished Processing Data ===============");

            // <SnippetReturnPipeline>
            return pipeline;
            // </SnippetReturnPipeline>
        }

        public IEstimator<ITransformer> BuildAndTrainModel(IDataView trainingDataView, IEstimator<ITransformer> pipeline)
        {
            // STEP 3: Create the training algorithm/trainer
            // Use the multi-class SDCA algorithm to predict the label using features.
            //Set the trainer/algorithm and map label to value (original readable state)
            // <SnippetAddTrainer>
            var trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features"))
                    .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));
            // </SnippetAddTrainer>

            // STEP 4: Train the model fitting to the DataSet
            Console.WriteLine($"=============== Training the model  ===============");

            // <SnippetTrainModel>
            _trainedModel = trainingPipeline.Fit(trainingDataView);

            // Extract Model Parameters of re-trained model
            // { Microsoft.ML.Data.MulticlassPredictionTransformer<Microsoft.ML.Trainers.MaximumEntropyModelParameters>}
            //var originalModelParameters = ((ISingleFeaturePredictionTransformer<object>)_trainedModel).Model as MaximumEntropyModelParameters;
            //var retrainedModelParameters = ((ISingleFeaturePredictionTransformer<object>)_trainedModel).Model as MaximumEntropyModelParameters;

            //// Inspect Change in Weights
            //var weightDiffs =
            //    originalModelParameters..Weights.Zip(
            //        retrainedModelParameters.Weights, (original, retrained) => original - retrained).ToArray();

            //Console.WriteLine("Original | Retrained | Difference");
            //for (int i = 0; i < weightDiffs.Count(); i++)
            //{
            //    Console.WriteLine($"{originalModelParameters.Weights[i]} | {retrainedModelParameters.Weights[i]} | {weightDiffs[i]}");
            //}
            //    trainingPipeline = pipeline.Append(_mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy("Label", "Features"))
            //.Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            //    _trainedModel = trainingPipeline.Fit(trainingDataView);
            // </SnippetTrainModel>
            Console.WriteLine($"=============== Finished Training the model Ending time: {DateTime.Now.ToString()} ===============");

            // (OPTIONAL) Try/test a single prediction with the "just-trained model" (Before saving the model)
            Console.WriteLine($"=============== Single Prediction just-trained-model ===============");

            // Create prediction engine related to the loaded trained model
            // <SnippetCreatePredictionEngine1>
            _predEngine = _mlContext.Model.CreatePredictionEngine<Learn22Base, IssuePredictionBaseScan>(_trainedModel);
            // </SnippetCreatePredictionEngine1>
            // <SnippetCreateTestIssue1>

            // </SnippetCreateTestIssue1>
            Learn22Base dataSetTest = dbContext.Learn22_testData.ToList()[1];
            // <SnippetPredict>
            var prediction = _predEngine.Predict(dataSetTest);
            // </SnippetPredict>

            // <SnippetOutputPrediction>
            Console.WriteLine($"=============== Single Prediction just-trained-model - Result: {prediction.isGood} ===============");
            // </SnippetOutputPrediction>

            // <SnippetReturnModel>
            return trainingPipeline;
            // </SnippetReturnModel>
        }

        public void Evaluate(DataViewSchema trainingDataViewSchema)
        {
            // STEP 5:  Evaluate the model in order to get the model's accuracy metrics
            Console.WriteLine($"=============== Evaluating to get model's accuracy metrics - Starting time: {DateTime.Now.ToString()} ===============");

            //Load the test dataset into the IDataView
            // <SnippetLoadTestDataset>
            //var testDataView = _mlContext.Data.LoadFromTextFile<GitHubIssue>(_testDataPath, hasHeader: true);
            // </SnippetLoadTestDataset>

            //Evaluate the model on a test dataset and calculate metrics of the model on the test data.
            // <SnippetEvaluate>
            var testMetrics = _mlContext.MulticlassClassification.Evaluate(_trainedModel.Transform(_testData));
            // </SnippetEvaluate>

            Console.WriteLine($"=============== Evaluating to get model's accuracy metrics - Ending time: {DateTime.Now.ToString()} ===============");
            // <SnippetDisplayMetrics>
            Console.WriteLine($"*************************************************************************************************************");
            Console.WriteLine($"*       Metrics for Multi-class Classification model - Test Data     ");
            Console.WriteLine($"*------------------------------------------------------------------------------------------------------------");
            Console.WriteLine($"*       MicroAccuracy:    {testMetrics.MicroAccuracy:0.###}");
            Console.WriteLine($"*       MacroAccuracy:    {testMetrics.MacroAccuracy:0.###}");
            Console.WriteLine($"*       LogLoss:          {testMetrics.LogLoss:#.###}");
            Console.WriteLine($"*       LogLossReduction: {testMetrics.LogLossReduction:#.###}");
            Console.WriteLine($"*************************************************************************************************************");
            // </SnippetDisplayMetrics>

            // Save the new model to .ZIP file
            // <SnippetCallSaveModel>
            SaveModelAsFile(_mlContext, trainingDataViewSchema, _trainedModel);
            // </SnippetCallSaveModel>
        }

        public static void PredictIssue()
        {
            //// <SnippetLoadModel>
            //ITransformer loadedModel = _mlContext.Model.Load(_modelPath, out var modelInputSchema);
            //// </SnippetLoadModel>

            //// <SnippetAddTestIssue>
            //Learn22 singleIssue = new Learn22() { Title = "Entity Framework crashes", Description = "When connecting to the database, EF is crashing" };
            //// </SnippetAddTestIssue>

            ////Predict label for single hard-coded issue
            //// <SnippetCreatePredictionEngine>
            //_predEngine = _mlContext.Model.CreatePredictionEngine<Learn22, IssuePredictionBaseScan>(loadedModel);
            //// </SnippetCreatePredictionEngine>

            //// <SnippetPredictIssue>
            //var prediction = _predEngine.Predict(singleIssue);
            //// </SnippetPredictIssue>

            //// <SnippetDisplayResults>
            //Console.WriteLine($"=============== Single Prediction - Result: {prediction.Area} ===============");
            //// </SnippetDisplayResults>
        }

        private static void SaveModelAsFile(MLContext mlContext, DataViewSchema trainingDataViewSchema, ITransformer model)
        {
            // <SnippetSaveModel>
            mlContext.Model.Save(model, trainingDataViewSchema, _modelPath);
            // </SnippetSaveModel>

            Console.WriteLine("The model is saved to {0}", _modelPath);
        }
    }
}
