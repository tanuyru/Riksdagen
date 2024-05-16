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
var outputDir = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\";
var emptyDir = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\empty\";
var failDir = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\failed\";

var copyInput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\";
var copyOutput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\organ\";

var csvInput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propscsv\";
//var propJsonData = new CsvImporter().ImportBaseFromCsv(csvInput);

//Console.WriteLine("Loaded " + propJsonData?.Count + " element from csv file " + csvInput);

//var dictionary = propJsonData.ToDictionary(m => m.DokumentId.Trim('"').ToLower(), m => m);
//new FileSelector().CopyAndCreate(copyInput, copyOutput, dictionary);

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
var modelInputFixed = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\organ\fixed\";

var modelOutputFixed = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\organ\fixed\correctorgan\";
var models = FileSelector.LoadFromDir<PropositionExportModel>(modelInputFixed);

var corrOrgan = models.Where(mod => mod.Organ == mod.GuessedOrgan || mod.Organ.Length <= 3 || mod.Organ.Contains("t.o.m")).ToList();
FileSelector.SaveToDir(corrOrgan, modelOutputFixed, r => r.DokumentId.ToLower() + ".json");