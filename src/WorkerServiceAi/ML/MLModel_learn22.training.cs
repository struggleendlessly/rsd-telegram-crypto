﻿// This file was auto-generated by ML.NET Model Builder.
using Microsoft.ML;
using Microsoft.ML.Data;

using WorkerServiceAi.DB;

namespace ML.WorkerServiceAi
{
    public class IssuePredictionBaseScan
    {
        [ColumnName("PredictedLabel")]
        public string didXXX;
    }
    public partial class MLModel_learn22
    {
        private readonly DBContext dbContext;
        IDataView trainData;
        IDataView testData;
        public MLModel_learn22(
            DBContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public void Train(string outputModelPath)
        {
            var mlContext = new MLContext(seed: 1);

            LoadIDataViewFromDatabase(mlContext);
            var model = RetrainModel(mlContext);
            SaveModel(mlContext, model, trainData, outputModelPath);
        }


        public void LoadIDataViewFromDatabase(MLContext mlContext)
        {
            var dataSet = 
                dbContext.
                ContractSourceCodeTrainDatas.
                Where(x=>!string.IsNullOrEmpty(x.didXXX) && !string.Equals(x.didXXX, "?")).
                Take(10).
                ToList();
            var data = mlContext.Data.LoadFromEnumerable(dataSet);

            var trainTestSplit = mlContext.Data.TrainTestSplit(data);
            trainData = trainTestSplit.TrainSet;
            testData = trainTestSplit.TestSet;
        }

        public static void SaveModel(MLContext mlContext, ITransformer model, IDataView data, string modelSavePath)
        {
            // Pull the data schema from the IDataView used for training the model
            DataViewSchema dataViewSchema = data.Schema;

            using (var fs = File.Create(modelSavePath))
            {
                mlContext.Model.Save(model, dataViewSchema, fs);
            }
        }

        public ITransformer RetrainModel(MLContext mlContext)
        {
            var pipeline = BuildPipeline(mlContext);

            var trainingPipeline = 
                pipeline.
                Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("Label", "Features")).
                //Append(mlContext.MulticlassClassification.CrossValidate.SdcaMaximumEntropy("Label", "Features")).
                Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = trainingPipeline.Fit(trainData);

            return model;
        }

        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "didXXX", outputColumnName: "Label")
                 .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "SourceCode", outputColumnName: "SourceCodeFeaturized"))
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "contractByteCode", outputColumnName: "contractByteCodeFeaturized"))
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "contractABI", outputColumnName: "contractABIFeaturized"))
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "totalSupply", outputColumnName: "totalSupplyFeaturized"))
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "divisor", outputColumnName: "divisorFeaturized"))
                .Append(mlContext.Transforms.Concatenate(
                    "Features",
                    "SourceCodeFeaturized"
                    //"contractByteCodeFeaturized", 
                    //"contractABIFeaturized", 
                    //"totalSupplyFeaturized", 
                    //"divisorFeaturized"
                    ));

            return pipeline;
        }
    }
}
