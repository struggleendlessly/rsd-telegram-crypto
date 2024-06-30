using Microsoft.ML.Trainers.FastTree;
using Microsoft.ML.Trainers;
using Microsoft.ML;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Azure.Core.HttpHeader;
using WorkerServiceAi.DB;

namespace WorkerServiceAi.ML
{
    public class Training: BaseML
    {
        public Training(DBContext dbContext):base(dbContext)
        {
            
        }
        public void CompareTrainerEvaluations()
        {

            // Create a transformation pipeline.
            //var featurizationPipeline = mlContext.Transforms.Text.FeaturizeText("Features", "SourceCode").
            //    .AppendCacheCheckpoint(mlContext);

            // Create a selection of learners.
            var sdcaTrainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    new SdcaLogisticRegressionBinaryTrainer.Options { NumberOfThreads = 8 });

            var fastTreeTrainer = mlContext.BinaryClassification.Trainers.FastTree(
                    new FastTreeBinaryTrainer.Options { NumberOfThreads = 1 });

            var ffmTrainer = mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine();

            // Fit the data transformation pipeline.
            var featurization = featurizationPipeline.Fit(trainData);
            var featurizedTrain = featurization.Transform(trainData);
            var featurizedTest = featurization.Transform(testData);

            // Fit the trainers.
            var sdca = sdcaTrainer.Fit(featurizedTrain);
            var fastTree = fastTreeTrainer.Fit(featurizedTrain);
            var ffm = ffmTrainer.Fit(featurizedTrain);

            // Evaluate the trainers.
            var sdcaPredictions = sdca.Transform(featurizedTest);
            var sdcaMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(sdcaPredictions);
            var fastTreePredictions = fastTree.Transform(featurizedTest);
            var fastTreeMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(fastTreePredictions);
            var ffmPredictions = sdca.Transform(featurizedTest);
            var ffmMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(ffmPredictions);

            //// Validate the results.
            //Common.AssertMetrics(sdcaMetrics);
            //Common.AssertMetrics(fastTreeMetrics);
            //Common.AssertMetrics(ffmMetrics);
        }
    }
}
