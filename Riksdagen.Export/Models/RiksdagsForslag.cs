using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Export.Models
{
    public class RiksdagsForslag
    {
        public string Text { get; set; } = default!;
        public List<Anslag>? Anslag { get; set; }
    }
}
