using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riksdagen.Import
{
    public class HtmlParser
    {
        public string ParseSummary(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var node = doc.DocumentNode.SelectSingleNode("//div[@id='page_1']");
            if (node != null)
            {
                Console.WriteLine("Unknown format, cant find page1");
                return null; // Unknown format?
            }
            return "Dorum Ipsom";
        }
    }
}
