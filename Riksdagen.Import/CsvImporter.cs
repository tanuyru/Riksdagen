using Riksdagen.Import.ExportedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import
{
    public class CsvImporter
    {
        public List<PropositionExportModel> ImportBaseFromCsv(string dir)
        {
            var models = new List<PropositionExportModel>();
            var files = Directory.GetFiles(dir);
            int totalLines = files.Length;
            foreach(var file in files)
            {
                var allRows = File.ReadAllLines(file);
                foreach(var row in allRows)
                {
                    try
                    {
                        var model = ReadModelFromLine(row);
                        if (model != null)
                        {
                            models.Add(model);
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }

            }
            Console.WriteLine("Read " + models.Count + "/"+totalLines+" models from " + dir);

            return models;
        }
        const int IdCol = 1;
        const int PropPerfixCol = 2;
        const int PropPostfixCol = 3;
        const int DokTypCOl = 4;
        const int OrganShortCol = 9;
        const int DokDateCol = 11;
        const int TitleCol = 13;
        const int StatusCol = 14;

        PropositionExportModel ReadModelFromLine(string line)
        {
            const char del = ',';
            var cols = line.Split(del);
            if (cols.Length <= 14)
            {
                return null;
            }
            var idCol = cols[IdCol];
            var propPrefixCol = cols[PropPerfixCol];
            var propPostFixCol = cols[PropPostfixCol];
            var dokTypCol = cols[DokTypCOl];
            var orgCol = cols[OrganShortCol];
            var dateCol = cols[DokDateCol];
            var statusCol = cols[StatusCol];

            // All documents should have Id right?
            if (string.IsNullOrEmpty(idCol))
            {
                Console.WriteLine("NO ID FOUND FOR ROW " + line);
                return null;
            }

            // This can maybe happen?
            if (string.IsNullOrEmpty(propPrefixCol))
            {
                Console.WriteLine("No prop prefix for line " + line);
            }
            
            var model = new PropositionExportModel();
            model.DokumentId = idCol;
            model.Status = statusCol;
            model.Beteckning = propPostFixCol;
            model.Rm = propPrefixCol;
            model.Organ = orgCol;
            model.DokumentTyp = dokTypCol;
            if (DateTime.TryParse(dateCol, out var dokDate))
            {
                model.DokDate = dokDate;
            }
            return model;
        }
    }
}
