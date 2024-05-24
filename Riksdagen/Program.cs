// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using Riksdagen;
using Riksdagen.Import;
using Riksdagen.Import.ExportedModels;
using Riksdagen.Import.Models;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

Console.WriteLine("Hello, World!");

List<string> inputDirs = Directory.GetDirectories(@"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\").ToList();
var inputDir = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propstxt\filtered\";
var outputDir = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\pipeline\pipelineoutput\";
var tempDir = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\pipeline\tempempty\";
var emptyDir = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\pipeline\empty\";
var failDir = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\pipeline\failed\";

var copyInput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\";
var copyOutput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\organ\";

var csvInput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propscsv\";
var doneOutput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\pipeline\pipelineoutput\";
var outputFile = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\pipeline\output.tsv";

var budgetOutput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\pipeline\budgets\";
var testFile = doneOutput + "\\..\\test.tsv";
var trainFile = doneOutput + "\\..\\train.tsv";

var inputFilter = @"C:\Users\whydo\Downloads\prop-2022-2025.html(1)\";
var outputFilter = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\prop2025htmlchange\";

Pipeline.FilterText(inputFilter, outputFilter, s => s.Contains("Förslag till lag om ändring"));
return;
Exporter.ExportTrainAndTestDataPerDep(doneOutput, new DateTime(1990, 10, 4), new DateTime(2010, 10, 8),
    @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\pipeline\perdep\");
return;
Exporter.ExportTrainAndTestData(doneOutput, new DateTime(1990, 10, 4), new DateTime(2010, 10, 8), testFile, trainFile);

return;

Console.WriteLine("Starting complete pipeline...");
Pipeline.RunSummaryPipeline(inputDirs.ToArray(),
    tempDir,
    outputDir,
    failDir,
    emptyDir,
    csvInput,
    false
    );
/*
var fileInput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\organ\";
var fileOutput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\organ\fixed\";
var allModels = FileSelector.LoadFromDir<PropositionExportModel>(fileInput);
var groups = allModels.GroupBy(m => m.Organ?.ToLower());
foreach(var g in groups.OrderByDescending(gr => gr.Count()))
{
    Console.WriteLine(g.Key+": "+g.Count());
}

FileSelector.FixOrgan(allModels);
Console.WriteLine("After fixing have-------------------------");
groups = allModels.GroupBy(m => m.Organ?.ToLower());
foreach (var g in groups.OrderByDescending(gr => gr.Count()))
{
    Console.WriteLine(g.Key + ": " + g.Count());
}

FileSelector.SaveToDir(allModels, fileOutput, m => m.DokumentId.Trim().ToLower() + ".json");

*/
/*
var modelInputFixed = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\";

var modelOutputFixed = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\organ\fixed\correctorgan\";
var models = FileSelector.LoadFromDir<PropositionExportModel>(modelInputFixed);

var corrOrgan = models.Where(mod => mod.Organ == mod.GuessedOrgan || mod.Organ.Length <= 3 || mod.Organ.Contains("t.o.m")).ToList();
FileSelector.SaveToDir(corrOrgan, modelOutputFixed, r => r.DokumentId.ToLower() + ".json");
*/