using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Riksdagen.Import
{
    public static class HtmlExtensions
    {
        public static HtmlNode GetBodyNode(this HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//body");
        }
        public static List<string> ParseSections(this HtmlNode rootNode)
        {
            return new List<string>();
        }
        public static Dictionary<double, HtmlDocument> SplitSections(this HtmlNode rootNode)
        {
            const string EmptyHtml = "<html><body></body></html>";
            Dictionary<double, HtmlDocument> result = new Dictionary<double, HtmlDocument>();
            HtmlDocument? curr = null;
            var currentNode = GetFirstElementOnFirstPage(rootNode);
            if (currentNode == null)
            {
                return result;
            }
            double currentSection = 0;
            do
            {
                var sectionNumber = currentNode.TryGetSectionNameSpan(currentSection);
                if (sectionNumber.HasValue && sectionNumber.Value > currentSection)
                {
                    Console.WriteLine("Found new section: " + sectionNumber.Value);
                    if (!result.TryGetValue(sectionNumber.Value, out var parentNode))
                    {
                        parentNode = new HtmlDocument();
                        parentNode.LoadHtml(EmptyHtml);
                        result.Add(sectionNumber.Value, parentNode);
                        curr = parentNode;
                        currentSection = sectionNumber.Value;
                    }
                }
                else if (curr != null)
                {
                    curr.GetBodyNode().AppendChild(currentNode.Clone());
                }
                var next = currentNode.GetNextElement();
                if (next == null)
                    break;
                do
                {
                    currentNode = currentNode.GetNextElement();
                }
                while (currentNode != null && currentNode.NodeType != HtmlNodeType.Element);
            } while (currentNode != null);
            return result;
        }
        public static double? TryGetSectionNameSpan(this HtmlNode node, double lastSection)
        {

                    var wholeSection = ParseMainSectionNumber(node.InnerText);
                    if (wholeSection.HasValue)
                    {
                        Console.WriteLine("Found whole section from " + node.InnerText);
                        return wholeSection.Value;
                    }
         
           

            var possibleParagraphNodes = node.SelectNodes("span");
            if (possibleParagraphNodes == null)
                return null;
            foreach (var possibleNode in possibleParagraphNodes)
            {
                var parsedSection = ParseSectionNumber(possibleNode.InnerText);
                if (parsedSection != null)
                {
                    var diff = parsedSection.Value - lastSection;
                    if (diff > 1.0)
                    {
                        Console.WriteLine("Wrong section found diff is " + diff + " lastSection " + lastSection + " parsed: " + parsedSection);
                    }
                    else
                    {
                        return parsedSection;

                    }
                }
            }

            return null;
        }
    
        public static double? TryGetSectionNameTable(this HtmlNode node)
        {
            if (node.Name == "table")
            {
                // Could be new section, lets investigate

                var possibleParagraphNodes = node.SelectNodes("/td/span");
                foreach(var possibleNode in possibleParagraphNodes)
                {
                    var parsedSection = ParseSectionNumber(possibleNode.InnerText);
                    if (parsedSection != null)
                    {
                        return parsedSection;
                    }
                }
            }
            return null;
        }
        public static int? ParseMainSectionNumber(string text)
        {
            var parts = text.Split(' ');
            if (parts.Length == 0) return null;
            if (int.TryParse(parts[0], out var wholeSectionNumber))
                return wholeSectionNumber;
            return null;
        }
        public static double? ParseSectionNumber(string text)
        {
           
            var parts = text.Split('.');
            double val = 0;
            for(int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out var number))
                {
                    return null; // Invalid
                }
                val += number / Math.Pow(10, i); 
            }
            return val != 0 ? val : null;
        }
        public static string? PropSummary(this HtmlNode parentNode)
        {
            return null;
        } 
        public static List<HtmlNode>? HtmlChildren(this HtmlNode node)
        {
            return node?.ChildNodes?.Where(n => n.NodeType == HtmlNodeType.Element).ToList();
        }
        public static HtmlNode? NextElementSibling(this HtmlNode node)
        {
            var nextSibling = node?.NextSibling;
            while(nextSibling != null && nextSibling.NodeType != HtmlNodeType.Element)
            {
                nextSibling = nextSibling.NextSibling;
            }
            return nextSibling?.NodeType == HtmlNodeType.Element ? nextSibling : null;
        }
        public static HtmlNode? GetNextElement(this HtmlNode sibling)
        {
            var nextElement = sibling.NextElementSibling();
            if (nextElement == null)
            {
                var parentNode = sibling.ParentNode?.ParentNode;

                Console.WriteLine("Finished with page id: " + parentNode?.Id+" for sibling "+sibling.GetHashCode());
                
                var nextParentSibling = parentNode?.NextElementSibling();
                while(nextParentSibling != null && (nextParentSibling.ChildNodes == null || nextParentSibling.HtmlChildren().Count == 0))
                {
                    nextParentSibling = nextParentSibling.NextElementSibling();
                }
                var selectedNode = nextParentSibling?.ChildNodes.FirstOrDefault(n => n.Id == "id_1");
                Console.WriteLine("Proceeding with next page that has " + selectedNode?.HtmlChildren()?.Count + " children");
                var finalElement = selectedNode?.HtmlChildren()?.FirstOrDefault();
                
                if (finalElement?.ParentNode.Id != "id_1")
                {
                    Console.WriteLine("Selected element that is not child of a id_1 element!?");
                }
                Console.WriteLine("Found new page: " + finalElement?.ParentNode?.ParentNode?.Id);
                return finalElement;
            }
            return nextElement;
        }
        public static HtmlNode? GetFirstElementOnFirstPage(this HtmlNode node)
        {
            var firstPage = node.SelectNodes("//div").SingleOrDefault(n => n.Id == "page_3");
            var subDivs = firstPage.SelectNodes("div");
            var id1Div = subDivs
                .SingleOrDefault(n => n.Id == "id_1");

            var htmlChildren = id1Div.HtmlChildren();
            return htmlChildren?.FirstOrDefault();
        }
        public static List<HtmlNode>? GetPageDivs(this HtmlNode node)
        {
            var firstPage = node.SelectSingleNode("//div[@id='page_1']");
            return firstPage?.ParentNode?.ChildNodes.ToList();
        }
    }
}
