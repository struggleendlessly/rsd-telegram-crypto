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
                    new FastTreeBinaryTrainer.Options { NumberOfThreads = 8 });

            var FastForestTrainer = mlContext.BinaryClassification.Trainers.FastForest(
                new FastForestBinaryTrainer.Options() {  NumberOfThreads = 8 });

            var GamTrainer = mlContext.BinaryClassification.Trainers.Gam(
                new GamBinaryTrainer.Options() {   });

            var LbfgsLogisticRegressionTrainer = mlContext.BinaryClassification.Trainers.LbfgsLogisticRegression(
                new LbfgsLogisticRegressionBinaryTrainer.Options() { NumberOfThreads = 8 });

            var LdSvmTrainer = mlContext.BinaryClassification.Trainers.LdSvm(
                new LdSvmTrainer.Options() {});

            var LinearSvmTrainer = mlContext.BinaryClassification.Trainers.LinearSvm(
                new LinearSvmTrainer.Options() {});

            var SgdNonCalibratedTrainer = mlContext.BinaryClassification.Trainers.SgdNonCalibrated(
                new SgdNonCalibratedTrainer.Options() {NumberOfThreads = 8});

            var ffmTrainer = mlContext.BinaryClassification.Trainers.FieldAwareFactorizationMachine();

            var priorTrainer = mlContext.BinaryClassification.Trainers.Prior();

            var SgdCalibratedTrainer = mlContext.BinaryClassification.Trainers.SgdCalibrated(
                new SgdCalibratedTrainer.Options() { NumberOfThreads = 8 });

            // Fit the data transformation pipeline.
            var featurization = featurizationPipeline.Fit(trainData);
            var featurizedTrain = featurization.Transform(trainData);
            var featurizedTest = featurization.Transform(testData);

            // Fit the trainers.
            var SgdCalibrated = SgdCalibratedTrainer.Fit(featurizedTrain);
            var prior = priorTrainer.Fit(featurizedTrain);
            var SgdNonCalibrated = SgdNonCalibratedTrainer.Fit(featurizedTrain);
            var LinearSvm = LinearSvmTrainer.Fit(featurizedTrain);
            var LdSvm = LdSvmTrainer.Fit(featurizedTrain);
            var lbfgsLogisticRegression = LbfgsLogisticRegressionTrainer.Fit(featurizedTrain);
            var Gam = GamTrainer.Fit(featurizedTrain);
            var fastForest = fastTreeTrainer.Fit(featurizedTrain);
            var sdca = sdcaTrainer.Fit(featurizedTrain);
            var fastTree = fastTreeTrainer.Fit(featurizedTrain);
            var ffm = ffmTrainer.Fit(featurizedTrain);

            // Evaluate the trainers.
            var SgdCalibratedPredictions = SgdCalibrated.Transform(featurizedTest);
            var SgdCalibratedMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(SgdCalibratedPredictions);

            var priorPredictions = prior.Transform(featurizedTest);
            var priorMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(priorPredictions);

            var SgdNonCalibratedPredictions = SgdNonCalibrated.Transform(featurizedTest);
            var SgdNonCalibratedMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(SgdNonCalibratedPredictions);

            var LinearSvmPredictions = LinearSvm.Transform(featurizedTest);
            var LinearSvmMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(LinearSvmPredictions);

            var LdSvmPredictions = LdSvm.Transform(featurizedTest);
            var LdSvmMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(LdSvmPredictions);
           
            var lbfgsLogisticRegressionPredictions = lbfgsLogisticRegression.Transform(featurizedTest);
            var lbfgsLogisticRegressionMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(lbfgsLogisticRegressionPredictions);
           
            var GamPredictions = Gam.Transform(featurizedTest);
            var GamMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(GamPredictions);

            var fastForestPredictions = fastForest.Transform(featurizedTest);
            var fastForestMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(fastForestPredictions);

            var sdcaPredictions = sdca.Transform(featurizedTest);
            var sdcaMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(sdcaPredictions);

            var fastTreePredictions = fastTree.Transform(featurizedTest);
            var fastTreeMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(fastTreePredictions);

            var ffmPredictions = sdca.Transform(featurizedTest);
            var ffmMetrics = mlContext.BinaryClassification.EvaluateNonCalibrated(ffmPredictions);
        }
    }
}
