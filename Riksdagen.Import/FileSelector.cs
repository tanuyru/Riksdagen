using Riksdagen.Import.ExportedModels;
using Riksdagen.Import.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Riksdagen.Import
{
    public class FileSelector
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
                    res.ParsedResult = obj;
                    if (!filterFunc(res))
                    {
                        continue;
                    }


                    var outputName = outputDir + Path.GetFileName(f);
                    var transformedJson = JsonSerializer.Serialize(res);
                    File.WriteAllText(outputName, transformedJson);
                    numCopied++;
                    // Console.WriteLine("Found file, "+numTotal+" and copied " + outputName);
                }
                catch (Exception ex)
                {
                    numFailed++;
                    Console.WriteLine("ERROR processing file: " + f);
                    Console.WriteLine(ex.ToString());
                }
            }
            Console.WriteLine("Done with " + numCopied + "/" + numTotal+" failed: "+numFailed);
        }
        public static PropositionExportModel TransformFunc(HtmlParsedResult input, string fileName, Dictionary<string, PropositionExportModel> dic)
        {
            var id = Path.GetFileNameWithoutExtension(fileName).ToLower();
            var existing = dic[id];
            existing.ParsedResult = input;
            return existing;
        }

      
        public static List<T> LoadFromDir<T>(string inputDir)
        {
            var l = new List<T>();
            foreach(var f in Directory.GetFiles(inputDir))
            {
                var txt = File.ReadAllText(f);
                var obj = JsonSerializer.Deserialize<T>(txt);
                l.Add(obj);
            }
            return l;
        }
        public static void SaveToDir<T>(List<T> list, string outputDir, Func<T,string> filenameFunc)
        {
            foreach(var t in list)
            {
                var fn = outputDir + filenameFunc(t);
                File.WriteAllText(fn, JsonSerializer.Serialize<T>(t));
            }
        }
        const string FiDep = "finansdepartementet";
        const string JuDep = "justitiedepartementet";
        const string SoDep = "socialdepartementet";
        const string UDDep = "utrikesdepartementet";
        const string UbDep = "utbildningsdepartementet";
        const string AmDep = "arbetsmarknadsdepartementet";
        const string KNDep = "klimat- och näringslivsdepartementet";
        const string LIDep = "landsbygds- och infrastrukturdepartementet";
        const string StBr = "statsrådsberedningen";
        const string FvDep = "försvarsdepartementet";
        const string KuDep = "kulturdepartementet";

        public static HashSet<string> AllDepartement = new HashSet<string>()
        {
            FiDep,
            JuDep,
            SoDep,
            UDDep,
            UbDep,
            AmDep,
            FvDep,
            KNDep,
            LIDep,
            StBr,
            KuDep,
        };
        public static int FixOrgan(List<PropositionExportModel> models)
        {
            var organsDictionary = new Dictionary<string, HashSet<string>>
            {
                { FiDep, new HashSet<string>() },
                { JuDep, new HashSet<string>() },
                { SoDep, new HashSet<string>() },
                { UDDep, new HashSet<string>() },
                { UbDep, new HashSet<string>() },
                { AmDep, new HashSet<string>() },
                { FvDep, new HashSet<string>() },
                { KNDep, new HashSet<string>() },
                { LIDep, new HashSet<string>() },
                { StBr, new HashSet<string>() },
                { KuDep, new HashSet<string>() },

            };
            // Finansdep
            organsDictionary[FiDep].Add("fi");
            organsDictionary[FiDep].Add("finansdepartementet-");

            // Justitie
            organsDictionary[JuDep].Add("ju");
            organsDictionary[JuDep].Add("justitiedepartement");
            organsDictionary[JuDep].Add("justitedepartementet");

            // Socialdep
            organsDictionary[SoDep].Add("s");
            organsDictionary[SoDep].Add("integrations- och jämställdhetsdepartementet");
            organsDictionary[SoDep].Add("civildepartementet");
            organsDictionary[SoDep].Add("kommunikationsdepartementet");
            // UD
            organsDictionary[UDDep].Add("ud");
            organsDictionary[UDDep].Add("-utrikesdepartementet");

            // Utbildning
            organsDictionary[UbDep].Add("u");
            organsDictionary[UbDep].Add("u t.o.m. 1 jan-07");
            organsDictionary[UbDep].Add("u t.o.m. -04");
            organsDictionary[UbDep].Add("utbildningsdepartementet)");

            // AM
            organsDictionary[AmDep].Add("a");

            // Försvar
            organsDictionary[FvDep].Add("fö");

            // Klimat & Näring
            organsDictionary[KNDep].Add("näringsdepartemetet");
            organsDictionary[KNDep].Add("näringsdepartementet");
            organsDictionary[KNDep].Add("n");
            organsDictionary[KNDep].Add("närings¶ och handelsdepartementet");
            organsDictionary[KNDep].Add("näringsdepartemetet");
            organsDictionary[KNDep].Add("miljö- och energidepartementet");
            organsDictionary[KNDep].Add("miljödepartementet");
            organsDictionary[KNDep].Add("m");
            organsDictionary[KNDep].Add("m  t.o.m. 1 jan-07");
            organsDictionary[KNDep].Add("m t.o.m. -04");
            organsDictionary[KNDep].Add("m");
            organsDictionary[KNDep].Add("närings- och handelsdepartementet");

            // Kultur
            organsDictionary[KuDep].Add("ku");
            organsDictionary[KuDep].Add("kud");
            organsDictionary[KuDep].Add("kud t.o.m. -04");


            // Landsbygd
            organsDictionary[LIDep].Add("l");
            organsDictionary[LIDep].Add("miljö- och naturresursdepartementet");
            organsDictionary[LIDep].Add("inrikesdepartementet");
            organsDictionary[LIDep].Add("miljö- och samhällsbyggnadsdepartementet");
            organsDictionary[LIDep].Add("jordbruksdepartementet");
            organsDictionary[LIDep].Add("jo");
            organsDictionary[LIDep].Add("jordbruksdepartemetet");


            // Statsrådsberedning
            organsDictionary[StBr].Add("sb");

            int numFixed = 0;
            int numRemoved = 0;
            foreach (var model in models.ToList())
            {
                if (model.Organ == null)
                    continue;
                model.Organ = model.Organ.ToLower();
                bool ok = true;
                if (!organsDictionary.ContainsKey(model.Organ))
                {
                    ok = false;
                    foreach(var kvp in organsDictionary)
                    {
                        if (kvp.Value.Contains(model.Organ))
                        {
                            model.GuessedOrgan = kvp.Key;
                            numFixed++;
                            ok = true;
                            break;
                        }
                    }
                }
                else
                {
                    model.GuessedOrgan = model.Organ;
                }
                if (!ok)
                {
                    Console.WriteLine("Removing " + model.Organ);
                    //models.Remove(model);
                    numRemoved++;
                }
            }

            Console.WriteLine("Fixed " + numFixed + ", removed: " + numRemoved);
            return numFixed;
        }


    }
}
