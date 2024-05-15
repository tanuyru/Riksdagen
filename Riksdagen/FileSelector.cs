using Riksdagen.Import.ExportedModels;
using Riksdagen.Import.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Riksdagen
{
    internal class FileSelector
    {
        public void CopyFiles<T>(string inputDir, string outputDir, Func<T, bool> filterFunc, Func<string,T> creatorFunc = null)
        {
            if (creatorFunc == null)
            {
                creatorFunc = (s) => JsonSerializer.Deserialize<T>(s);
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            int numTotal = 0;
            int numCopied = 0;
            int numFailed = 0;
            foreach(var f in Directory.GetFiles(inputDir))
            {
                numTotal++;
                try
                {
                    var jsonText = File.ReadAllText(f);
                    var obj = creatorFunc(jsonText);
                    if (filterFunc(obj))
                    {
                        var outputName = outputDir + Path.GetFileName(f);
                        File.Copy(f, outputName, true);
                        numCopied++;
                    }
                }
                catch(Exception ex)
                {
                    numFailed++;
                    Console.WriteLine("ERROR processing file: " + f);
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        public void CopyAndCreate(string inputDir, string outputDir, 
            Dictionary<string, PropositionExportModel> dictionary,
            Func<PropositionExportModel, bool> filterFunc = null)
        {
            Func<string, HtmlParsedResult> creatorFunc = null;
            if (creatorFunc == null)
            {
                creatorFunc = (s) => JsonSerializer.Deserialize<HtmlParsedResult>(s);
            }

            if (filterFunc == null)
            {
                filterFunc = (f) => !string.IsNullOrEmpty(f.Organ);
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            int numTotal = 0;
            int numCopied = 0;
            int numFailed = 0;
            foreach (var f in Directory.GetFiles(inputDir))
            {
                numTotal++;
                try
                {
                    var jsonText = File.ReadAllText(f);
                    var id = Path.GetFileNameWithoutExtension(f).ToLower();

                    var obj = creatorFunc(jsonText);
                    var res = dictionary[id]; // just throw exception

                    if (!string.IsNullOrEmpty(res.Organ))
                    { 
                        res.ParsedResult = obj;

                    var outputName = outputDir + Path.GetFileName(f);
                        var transformedJson = JsonSerializer.Serialize(res);
                        File.WriteAllText(outputName, transformedJson);
                        numCopied++;
                        Console.WriteLine("Found file, "+numTotal+" and copied " + outputName);
                    }
                }
                catch (Exception ex)
                {
                    numFailed++;
                    Console.WriteLine("ERROR processing file: " + f);
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine("Done with " + numCopied + "/" + numTotal);
        }
        public static PropositionExportModel TransformFunc(HtmlParsedResult input, string fileName, Dictionary<string, PropositionExportModel> dic)
        {
            var id = Path.GetFileNameWithoutExtension(fileName).ToLower();
            var existing = dic[id];
            existing.ParsedResult = input;
            return existing;
        }

      
    }
}
