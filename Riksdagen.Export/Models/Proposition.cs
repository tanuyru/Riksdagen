using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Export.Models
{
    public class Proposition
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

        public string Status { get; set; } = default!;
        public DateTime? DokDate { get; set; } = default!;

        public string PropName { get; set; } = default!;

        public string? Summary { get; set; }


        public List<RiksdagsForslag> Suggestions { get; set; } = new List<RiksdagsForslag>();

    }
}
