using Microsoft.ML;
using Microsoft.ML.Data;

using WorkerServiceAi.DB;

namespace WorkerServiceAi.ML
{
    public class BaseML
    {
        private readonly DBContext dbContext;
        public IDataView data;
        public IDataView trainData;
        public IDataView testData;
        public EstimatorChain<ColumnConcatenatingTransformer> featurizationPipeline;

        public MLContext mlContext = new MLContext(seed: 1);
        public BaseML(DBContext dbContext)
        {
            this.dbContext = dbContext;

            var dataSetTrain =
                dbContext.
                ContractSourceCodes.
                Where(x => !string.IsNullOrEmpty(x.didXXX) && !string.Equals(x.didXXX, "?") && x.Id != 2 && x.Id != 3 && x.Id != 5 && x.Id != 6).
                //Take(10).
                ToList();

            var dataSetTest =
                dbContext.
                ContractSourceCodes.
                Where(x => x.Id == 2 || x.Id == 3 || x.Id == 5 || x.Id == 6).
                ToList();


            trainData = mlContext.Data.LoadFromEnumerable(dataSetTrain);
            //var trainTestSplit = mlContext.Data.TrainTestSplit(data);
            //trainData = trainTestSplit.TrainSet;
            testData = mlContext.Data.LoadFromEnumerable(dataSetTest);

            featurizationPipeline = 
                mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "isGood", outputColumnName: "Label").
                Append(mlContext.Transforms.Conversion.MapKeyToValue("Label")).
                Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "SourceCode", outputColumnName: "SourceCodeFeaturized")).
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "contractByteCode", outputColumnName: "contractByteCodeFeaturized"))
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "contractABI", outputColumnName: "contractABIFeaturized"))
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "totalSupply", outputColumnName: "totalSupplyFeaturized"))
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "divisor", outputColumnName: "divisorFeaturized"))
                Append(mlContext.Transforms.Concatenate(
                    "Features",
                    "SourceCodeFeaturized"
                    //"contractByteCodeFeaturized", 
                    //"contractABIFeaturized", 
                    //"totalSupplyFeaturized", 
                    //"divisorFeaturized"
                    ));
        }
    }
}
