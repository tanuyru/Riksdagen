using Riksdagen.Import.ExportedModels;
using System.Text.Json.Serialization;

namespace Riksdagen.Import
{
    public class Importer
    {
        public static List<PropositionExportModel> GetExportModels(string dir)
        {
            var docs = ImportFromDirectory(dir);
            return docs.Select(TransformModel).ToList();
        }
        public static List<Dokumentstatus> ImportFromDirectory(string dir)
        {
            List<Dokumentstatus> parsedDoks = new List<Dokumentstatus>();
            var allFiles = Directory.GetFiles(dir);
            foreach (var file in Directory.GetFiles(dir).Take(1))
            {
                try
                {
                    var jsonText = File.ReadAllText(file);
                    var dokStatus = System.Text.Json.JsonSerializer.Deserialize<PropJson>(jsonText);
                    if (dokStatus?.dokumentstatus != null)
                    {
                        parsedDoks.Add(dokStatus.dokumentstatus);

                    }
                    else
                    {
                        Console.WriteLine("Couldnt deserialize " + jsonText + " to docstatus");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed at parsing dokumentstatus for file: " + file+": "+ex.ToString());
                }
            }
            Console.WriteLine("Imported " + parsedDoks.Count + " from " + allFiles.Count() + " files");
            return parsedDoks;
        }

        public static List<Dokumentstatus> FilterByHtmlContent(List<Dokumentstatus> list, string filterString)
        {
            return list.Where(s => s.dokument?.html != null && s.dokument.html.Contains(filterString)).ToList();
        }

        public static List<PropositionExportModel> Transform(List<Dokumentstatus> dokumentStatuses)
        {
            return dokumentStatuses.Select(d => TransformModel(d)).ToList();
        }

        public static PropositionExportModel TransformModel(Dokumentstatus dok)
        {
            var export = new PropositionExportModel()
            {
                Organ = dok.dokument.organ,
                DokumentId = dok.dokument.dok_id,
                DokDate = !string.IsNullOrEmpty(dok.dokument.datum) ? DateTime.Parse(dok.dokument.datum) : null,
                Title = dok.dokument.titel,
                Status = dok.dokument.status,
                DokumentTyp = dok.dokument.doktyp,
                Beteckning = dok.dokument.beteckning,
                Rm = dok.dokument.rm,
                HtmlFormat = dok.dokument.htmlformat,
                UnformattedHtml = dok.dokument.html,

            };

            return export;
        }
    }
}
