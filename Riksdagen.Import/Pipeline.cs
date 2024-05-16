using Riksdagen.Import.ExportedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import
{
    public class Pipeline
    {
        // Easier use
        public static void RunSummaryPipeline(
            string inputDir,
            string outputDir,
            string csvInputDir)
        {
            var emptyDir = Path.Combine(outputDir, "empties\\");
            var failDir = Path.Combine(outputDir, "failed\\");
            var tempEmpty = Path.Combine(outputDir, "tempdir\\");
            RunSummaryPipeline(
                new[] { inputDir },
                tempEmpty,
                outputDir,
                failDir,
                emptyDir,
                csvInputDir,
                false);
        }
        /// <summary>
        /// Reads summary from propsitions .txt format and outputs models with dokument meta-data as well as their summary.
        /// Filters any proposition not containing summary.
        /// </summary>
        /// <param name="inputDirs">List of directory where to find the .txt files</param>
        /// <param name="tempDir">A temporary directory should be empty</param>
        /// <param name="outputDir">Final dir where end .json files end up</param>
        /// <param name="failDir">Copy files that filed the pipeline here</param>
        /// <param name="emptyDir">Copy files where the summary nor sections where read, but didnt throw any exception.</param>
        /// <param name="csvInputDir">Input dir of all csv-files containing meta-data about the propositions</param>
        /// <param name="removeSourceOnSuccess">Set to true if you want the original .txt files to be removed when successfully parsed.</param>
        public static void RunSummaryPipeline(
            string[] inputDirs,
            string tempDir, 
            string outputDir, 
            string failDir,
            string emptyDir,
            string csvInputDir,
            bool removeSourceOnSuccess = false)
        {
            var parser = new Parser();

            // Parser create its own dirs internally if needed so only need to create this here.
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Saves HtmlParsedResult (Summary and possible sections-info if parsed successfully)
            parser.ParseAndSave(inputDirs, tempDir, failDir, emptyDir, removeSourceOnSuccess);
            // Load data dictionary for meta data from csv-files
            var csvImporter = new CsvImporter();
            var propJsonData = csvImporter.ImportBaseFromCsv(csvInputDir);

            Console.WriteLine("Loaded " + propJsonData.Count + " element from csv file " + csvInputDir);

            var dictionary = propJsonData.ToDictionary(m => m.DokumentId.Trim('"').ToLower(), m => m);

            // Match HtmlParsedResults with data from the CSV-file to create final model
            var fileSelector = new FileSelector();
            fileSelector.CopyAndCreate(tempDir, outputDir, dictionary, m => !string.IsNullOrEmpty(m?.ParsedResult?.Summary));

            ClearDir(tempDir);

            var allData = FileSelector.LoadFromDir<PropositionExportModel>(outputDir); // Read finished models
            FileSelector.FixOrgan(allData); // Sets the GuessOrgan by guessing which department it would be in today
            TagHelper.TagByRegering(allData); // Tag data by regering and left right center;

            ClearDir(outputDir);

            Exporter.ExporCsvtModels(outputDir+"models.tsv", allData);


            // Save to output
        }
        static void ClearDir(string inputDir)
        {
            foreach(var f in Directory.GetFiles(inputDir))
            {
                File.Delete(f);
            }
        }
    }
}
