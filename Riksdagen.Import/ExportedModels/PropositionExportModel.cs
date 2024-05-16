﻿using Riksdagen.Import.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import.ExportedModels
{
    public interface ITaggable
    {
        void ApplyTag(string tagType, string tagValue);
    }
    public interface IDatable
    {
        DateTime? DokDate { get; set; }
    }

    public class PropositionExportModel : ITaggable, IDatable
    {
        public void ApplyTag(string tagType, string tagValue)
        {
            if (Tags == null)
                Tags = new Dictionary<string, string>();
            Tags.Remove(tagType);
            Tags.Add(tagType, tagValue);
        }
        /// <summary>
        /// Unique between all documents?
        /// </summary>
        public string DokumentId { get; set; } = default!;

        /// <summary>
        /// Which "Utskott" ?
        /// </summary>
        public string? Organ { get; set; }

        public string? GuessedOrgan { get; set; }

        public string DokumentTyp { get; set; } = default!;

        public string Title { get; set; } = default!;

        public string HtmlFormat { get; set; } = default!;

        public string UnformattedHtml { get; set; } = default!;
        public HtmlParsedResult? ParsedResult { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime? DokDate { get; set; } = default!;

        public Dictionary<string, string>? Tags { get; set; } = default!;
        // These two together seems to create another unique id: [Rm]:[Beteckning].
        // Can probably be used to link to voting on this so saving...
        public string Rm { get; set; } = default!;  
        public string Beteckning { get; set; } = default!;
    }
}
