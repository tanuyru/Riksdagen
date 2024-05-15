using Riksdagen.Import.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import.ExportedModels
{
    public class PropositionExportModel
    {
        /// <summary>
        /// Unique between all documents?
        /// </summary>
        public string DokumentId { get; set; } = default!;

        /// <summary>
        /// Which "Utskott" ?
        /// </summary>
        public string? Organ { get; set; }

        public string DokumentTyp { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string HtmlFormat { get; set; } = default!;

        public string UnformattedHtml { get; set; } = default!;
        public HtmlParsedResult? ParsedResult { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime? DokDate { get; set; } = default!;


        // These two together seems to create another unique id: [Rm]:[Beteckning].
        // Can probably be used to link to voting on this so saving...
        public string Rm { get; set; } = default!;  
        public string Beteckning { get; set; } = default!;
    }
}
