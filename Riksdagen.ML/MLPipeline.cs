using Microsoft.ML.Transforms.Text;
using Microsoft.ML;
using Riksdagen.ML.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML.Data;
using Microsoft.ML.TorchSharp;
using Microsoft.ML.TorchSharp.NasBert;
using System.Data;
using System.Reflection.Emit;

namespace Riksdagen.ML
{
    internal class MLPipeline
    {
        public class PropositionPrediction
        {
            [ColumnName("PredictionLabel")]
            public float Prediction { get; set; }

            public float Score { get; set; }
            public float Label { get; set; }
        }
        public static void RunPipeline()
        {
            var mlContext = new MLContext();
            IDataView trainingData = mlContext.Data.LoadFromTextFile<PropositionModel>("train.tsv", hasHeader: true);

            var trainTest = mlContext.Data.TrainTestSplit(trainingData, testFraction: 0.6);


            var estimator = mlContext.Transforms.Text.FeaturizeText(outputColumnName: "Features", inputColumnName: nameof(PropositionModel.Summary))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", featureColumnName: "Features"));

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(
                            outputColumnName: "Label",
                            inputColumnName: "Label")
                        .Append(mlContext.MulticlassClassification.Trainers.TextClassification(
                            labelColumnName: "Label",
                            sentence1ColumnName: "Summary",
                            architecture: BertArchitecture.Roberta))
                        .Append(mlContext.Transforms.Conversion.MapKeyToValue(
                            outputColumnName: "PredictedLabel",
                            inputColumnName: "PredictedLabel"));


            Console.WriteLine("Startint training of model....");
            // Train the model
            var model = estimator.Fit(trainingData);
            Console.WriteLine("Done traininng model...");
            // Load the test data
            IDataView testData = mlContext.Data.LoadFromTextFile<PropositionModel>("test.tsv", hasHeader: true);

            // Evaluate the model
            var predictions = model.Transform(testData);

            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(predictions, "Label");
            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
            // Print the evaluation metrics
        }
    }
}
