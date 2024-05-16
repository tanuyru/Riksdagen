using Riksdagen.Import.ExportedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import
{
    public class Exporter
    {

        public static void ExporCsvtModels(string outputPath, List<PropositionExportModel> models, 
            string seperator = "\t")
        {
            StringBuilder sb = new StringBuilder();
            // Create header
            sb.Append("Id")
                   .Append(seperator)
                   .Append("Date")
                   .Append(seperator)
                   .Append("Regering")
                   .Append(seperator)
                   .Append("LeftRight")
                   .Append(seperator)
                   .Append("Organ")
                   .Append(seperator)
                   .Append("GuessedOrgan")

                   .Append(seperator)
                   .Append("IsRight")
                   .Append(seperator)
                   .Append("Summary")
                   .AppendLine();
            foreach (var m in models)
            {
                sb.Append(m.DokumentId)
                    .Append(seperator)
                    .Append(m.DokDate.Value)
                    .Append(seperator)
                    .Append(m.Tags[TagHelper.regeringsName])
                    .Append(seperator)
                    .Append(m.Tags[TagHelper.leftRightCenter])
                    .Append(seperator)
                    .Append(m.Organ)
                    .Append(seperator)
                    .Append(m.GuessedOrgan)
                         .Append(seperator)
                    .Append(m.IsRight().ToString())
                    .Append(seperator)
                    .Append(m.ParsedResult.GetCleanSummary())
               
                    .AppendLine();
            }
            var csvData = sb.ToString();
            if (csvData.Length > 0)
            {
                File.WriteAllText(outputPath, csvData);
                Console.WriteLine("Created output file " + outputPath+" from "+models.Count+" models");
            }
        }

        public static void ExportFromTo(string inputPath, string outputFileName)
        {
            var models = FileSelector.LoadFromDir<PropositionExportModel>(inputPath);
            ExporCsvtModels(outputFileName, models);
        }

        public static void ExportTrainAndTestData(string inputPath, DateTime startDate, DateTime splitDate, string testFileName = "test.tsv", string trainFileName = "train.tsv")
        {
            var models = FileSelector.LoadFromDir<PropositionExportModel>(inputPath);
            var train = models.Where(m => m.DokDate.Value >= startDate && m.DokDate.Value < splitDate).ToList();
            var test = models.Where(m => m.DokDate.Value >= splitDate).ToList();

            ExporCsvtModels(trainFileName, train);
            ExporCsvtModels(testFileName, test);
        }
    }
}
