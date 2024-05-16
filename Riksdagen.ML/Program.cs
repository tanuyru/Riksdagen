// Create a new MLContext
using Microsoft.ML;
using Riksdagen.ML.Models;

var mlContext = new MLContext();

// Load the data from the tsv file
var dataPath = Path.Combine(Environment.CurrentDirectory, "data.tsv");
var dataView = mlContext.Data.LoadFromTextFile<PropositionModel>(dataPath, separatorChar: '\t', hasHeader: true);

// Split the data into training and test sets
var splitData = mlContext.Data.TrainTestSplit(dataView, 0.1);

Console.WriteLine(splitData.TrainSet + " " + splitData.TestSet);
// Create a pipeline to train the model