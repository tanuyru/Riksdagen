using Riksdagen.Import.Models;
using Riksdagen.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Riksdagen
{
    internal class Parser
    {
        public void ParseAndSave(string[] inputDirs, string outputDir, string failDir, string emptyDir,
            bool removeSourceOnSuccess = false)
        {

            if (!string.IsNullOrEmpty(emptyDir) && Directory.Exists(emptyDir))
            {
                Directory.CreateDirectory(emptyDir);
            }
            if (!string.IsNullOrEmpty(failDir) && !Directory.Exists(failDir))
            {
                Directory.CreateDirectory(failDir);
            }
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            int fileCounter = 0;
            int successCounter = 0;
            foreach (var dir in inputDirs.Where(d => d.ToLower() != emptyDir?.ToLower() && d.ToLower() != failDir?.ToLower() && d.ToLower() != outputDir.ToLower()))
            {


                foreach (var file in Directory.GetFiles(dir))
                {
                    Console.WriteLine("Processing " + file);
                    try
                    {
                        var lines = File.ReadAllLines(file);
                        if (lines.Any(l => !l.Contains("Förslag till riksdagsbeslut")))
                        {
                            continue;
                        }
                        fileCounter++;

                        var parser = new PropositionParserTxt(lines);
                        var sections = parser.ParseSections();
                        var summary = parser.GetSummary();

                        if (sections == null)
                        {
                            //Console.WriteLine("ParseSections returned null, no index found?");
                        }

                       // Console.WriteLine($"Found {sections?.Count} sections, {sections?.Count(s => s.Text != null)} non-null");

                        var res = new HtmlParsedResult();
                        res.Summary = summary;
                        res.Sections = sections;
                        var outputFilename = outputDir + Path.GetFileNameWithoutExtension(file) + ".json";
                        if ((res.Sections == null || res.Sections.Count == 0) &&
                            (string.IsNullOrEmpty(res.Summary)))
                        {
                            if (!string.IsNullOrEmpty(emptyDir))
                            {
                                File.Copy(file, emptyDir + Path.GetFileName(file), true);
                                Console.WriteLine("File was empty moved to empty folder");
                            }
                           
                        }
                        else
                        {
                            File.WriteAllText(outputFilename, JsonSerializer.Serialize(res));
                            Console.WriteLine("Successfully wrote file " + outputFilename + " to disk");
                            successCounter++;
                            if (removeSourceOnSuccess)
                            {
                                File.Delete(file);
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error while processingfile " + file);
                        Console.WriteLine(ex.ToString());
                        File.Copy(file, failDir + Path.GetFileName(file), true);
                    }
                }
            }
            Console.WriteLine("Successfully parsed " + successCounter + " from " + fileCounter + " files processed");
        }
    }
}
