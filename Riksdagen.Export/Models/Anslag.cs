using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Export.Models
{
    public class Anslag
    {
        public string Key { get; set; } = default!;
        public string? RowId { get; set; }
        public double Amount { get; set; }
        public int UtgiftsOmradeId { get; set; }

        /// <summary>
        /// Budget for the future?
        /// </summary>
        public AnslagsBemyndigande? Bemyndigande { get; set; }
    }
}
