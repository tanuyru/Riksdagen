using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import.Models
{
    public class DocSection
    {
        public string Id { get; set; } = default!;
        public double SectionNumber { get; set; }
        public string Title { get; set; } = default!;
        public string Text { get; set; } = default!;
        public int Page { get; set; } = default!;
    }
}
