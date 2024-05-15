// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
using Riksdagen;
using Riksdagen.Import;
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

new Parser().ParseAndSave(inputDirs.ToArray(), outputDir, failDir, emptyDir, false);