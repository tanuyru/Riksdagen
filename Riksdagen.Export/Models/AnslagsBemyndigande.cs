using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Export.Models
{
    public class AnslagsBemyndigande
    {
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = default!;
        public double Amount { get; set; }
    }
}
