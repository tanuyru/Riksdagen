// See https://aka.ms/new-console-template for more information
using HtmlAgilityPack;
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

if (!Directory.Exists(emptyDir))
{
    Directory.CreateDirectory(emptyDir);
}
if (!Directory.Exists(failDir))
{
    Directory.CreateDirectory(failDir);
}
if (!Directory.Exists(outputDir))
{
    Directory.CreateDirectory(outputDir);
}
foreach (var dir in inputDirs.Where(d => d.ToLower() != emptyDir.ToLower() && d.ToLower() != failDir.ToLower() && d.ToLower() != outputDir.ToLower()))
{

    
    foreach (var file in Directory.GetFiles(dir))
    {
        Console.WriteLine("Processing " + file);
        try
        {
            var lines = File.ReadAllLines(file);
            var parser = new PropositionParserTxt(lines);
            var sections = parser.ParseSections();
            var summary = parser.GetSummary();

            if (sections == null)
            {
                Console.WriteLine("ParseSections returned null, no index found?");
            }

            Console.WriteLine($"Found {sections?.Count} sections, {sections?.Count(s => s.Text != null)} non-null");

            var res = new HtmlParsedResult();
            res.Summary = summary;
            res.Sections = sections;
            var outputFilename = outputDir + Path.GetFileNameWithoutExtension(file) + ".json";
            if ((res.Sections == null || res.Sections.Count == 0) &&
                (string.IsNullOrEmpty(res.Summary))) 
            {
                File.Copy(file, emptyDir + Path.GetFileName(file));
                Console.WriteLine("File was empty moved to empty folder");
            }
            else
            {
                File.WriteAllText(outputFilename, JsonSerializer.Serialize(res));
                Console.WriteLine("Successfully wrote file " + outputFilename + " to disk");
            }
         

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while processingfile " + file);
            Console.WriteLine(ex.ToString());
            File.Copy(file, failDir + Path.GetFileName(file));
        }
    }
}