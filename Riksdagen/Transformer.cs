using Riksdagen.Export.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen
{
    internal class Transformer
    {
        public static List<Proposition> Transform(List<Dokumentstatus> dokumentStatuses)
        {
            return dokumentStatuses.Select(d => TransformModel(d)).ToList();
        }

        public static Proposition TransformModel(Dokumentstatus dok)
        {
            var export = new Proposition()
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
