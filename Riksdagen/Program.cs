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

foreach (var dir in inputDirs)
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
                Console.WriteLine("ParseSections returned null");
                continue;
            }
            if (summary?.Length < 10)
            {
                summary = "DEFAULT SUMMARY OF TEN OR MORE CHARS";
            }
            Console.WriteLine($"Found {sections.Count} sections, {sections.Count(s => s.Text != null)} non-null, summary: " + summary?.Substring(0, 10));

            var res = new HtmlParsedResult();
            res.Summary = summary;
            res.Sections = sections;
            var outputFilename = outputDir + Path.GetFileNameWithoutExtension(file) + ".json";
            File.WriteAllText(outputFilename, JsonSerializer.Serialize(res));
            Console.WriteLine("Successfully wrote file " + outputFilename + " to disk");

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while processingfile " + file);
            Console.WriteLine(ex.ToString());
        }
    }
}