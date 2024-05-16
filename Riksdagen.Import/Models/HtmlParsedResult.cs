using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import.Models
{
    public class HtmlParsedResult
    {
        public List<DocSection>? Sections { get; set; } = null;
        public string? Summary { get; set; }
        public string? Footer { get; set; }
        public string DokumentId { get; set; }
        public string? GetCleanSummary()
        {
            if (Summary == null)
                return null;
            return Summary.Replace(System.Environment.NewLine, " ");
        }
    }

    
}
