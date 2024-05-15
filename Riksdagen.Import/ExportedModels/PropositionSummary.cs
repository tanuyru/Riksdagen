using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import.ExportedModels
{
    public class PropositionSummary
    {
        public string DocumentId { get; set; } = default!;
        public string Label { get; set; } = default!;
        public string Summary { get; set; } = default!;
    }
}
