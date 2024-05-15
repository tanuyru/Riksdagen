using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import.Models
{
    public class HtmlParsedResult
    {
        public List<DocSection> Sections { get; set; } = new List<DocSection>();
        public string? Summary { get; set; }
    }

    
}
