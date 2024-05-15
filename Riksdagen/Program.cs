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
var copyOutput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propositionertxt\parsed\summary\organ";

var csvInput = @"C:\Users\whydo\OneDrive\Documentos\ml\riksdagen\propscsv\";
var propJsonData = new CsvImporter().ImportBaseFromCsv(csvInput);

Console.WriteLine("Loaded " + propJsonData?.Count + " element from csv file " + csvInput);

var dictionary = propJsonData.ToDictionary(m => m.DokumentId.ToLower(), m => m);
new FileSelector().CopyAndCreate(copyInput, copyOutput, dictionary);

