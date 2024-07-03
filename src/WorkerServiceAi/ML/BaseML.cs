using Microsoft.ML;
using Microsoft.ML.Data;

using WorkerServiceAi.DB;

using static Azure.Core.HttpHeader;

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
                ContractSourceCodeTrainDatas.
                Where(x => !string.IsNullOrEmpty(x.didXXX) 
                        && !string.Equals(x.didXXX, "?")
                        && !string.IsNullOrEmpty(x.byteCode)
                        ).
                //Take(1).
                ToList();

            var dataSetTest =
                dbContext.
                ContractSourceCodeTestDatas.
                ToList();

            foreach (var item in dataSetTrain)
            {
                var a = item.byteCode.Remove(0, 2).Chunk(4).Select(vc => new string(vc));
                var b = string.Join(" ", a);
                item.byteCode = b;
            }

            foreach (var item in dataSetTest)
            {
                var a = item.byteCode.Remove(0, 2).Chunk(4).Select(vc => new string(vc));
                var b = string.Join(" ", a);
                item.byteCode = b;
            }

            trainData = mlContext.Data.LoadFromEnumerable(dataSetTrain);
            //var trainTestSplit = mlContext.Data.TrainTestSplit(data);
            //trainData = trainTestSplit.TrainSet;
            testData = mlContext.Data.LoadFromEnumerable(dataSetTest);

            featurizationPipeline =
                mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "isGood", outputColumnName: "Label").
                Append(mlContext.Transforms.Conversion.MapKeyToValue("Label")).
                Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "SourceCode", outputColumnName: "SourceCodeFeaturized")).
                //Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "byteCode", outputColumnName: "byteCodeFeaturized")).
                //Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "ABI", outputColumnName: "ABIFeaturized")).
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "totalSupply", outputColumnName: "totalSupplyFeaturized"))
                //.Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "divisor", outputColumnName: "divisorFeaturized"))
                Append(mlContext.Transforms.Concatenate(
                    "Features",
                    "SourceCodeFeaturized"
                    //"byteCodeFeaturized",
                    //"ABIFeaturized"
                    //"totalSupplyFeaturized", 
                    //"divisorFeaturized"
                    ));
        }
    }
}
