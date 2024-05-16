using Riksdagen.Import.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Riksdagen.Import
{
    public class PropositionParserTxt
    {
        public static List<string> RemoveEmptyRows(IEnumerable<string> rows)
        {
            return rows.Where(r => !string.IsNullOrEmpty(r.Trim())).ToList();
        }

        public static string? GuessFooter(IEnumerable<string> rows)
        {
            var grouped = rows.GroupBy(r => r);
            return grouped.OrderByDescending(grouped => grouped.Count())
                .Where(g => g.Key.Length > 5)
                .Select(g => g.Key).FirstOrDefault();
        }

        private List<string> currentRows;
        private string? pageFooter;
        public PropositionParserTxt(IEnumerable<string> rows)
        {
            currentRows = RemoveEmptyRows(rows.ToList());
            pageFooter = GuessFooter(currentRows);
        }

        List<string> summaryTitles = new List<string>()
        {
            "ropositionens liuvudsakliga innehåll",
            "propositionens huvudsakliga innehåll",
        };
        public string GetSummary()
        {
            for(int i = 0; i < currentRows.Count; i++)
            {
                var row = currentRows[i].ToLower().Trim();
                if (row.Length < 3)
                    continue;
                if (summaryTitles.Any(row.Contains))
                {
                    return ReadSummary(currentRows, i+1);
                }
                else if (row.StartsWith("propositionens"))
                {
                    if (currentRows.Count > i + 1)
                    {
                        var nextRow = currentRows[i + 1].ToLower();
                        if (nextRow.StartsWith("huvudsakliga innehåll") || nextRow.StartsWith("liuvudsakliga innehåll"))
                        {
                            return ReadSummary(currentRows, i + 2);
                        }
                        else if (nextRow.StartsWith("huvudsakliga") && currentRows.Count > i + 2)
                        {
                            var nextNextRow = currentRows[i + 3].ToLower();
                            if (nextNextRow.StartsWith("innehåll") || nextNextRow.StartsWith("innehåll"))
                            {
                                return ReadSummary(currentRows, i + 3);
                            }
                        }
                    }
                    else if (currentRows.Count > i + 2)
                    {
                        var nextRow = currentRows[i + 2].ToLower();
                        if (nextRow.StartsWith("huvudsakliga innehåll") || nextRow.StartsWith("liuvudsakliga innehåll"))
                        {
                            return ReadSummary(currentRows, i + 2);
                        }
                    }
                }
            }
            return null;
        }
        string ReadSummary(List<string> rows, int startIdx)
        {
            // Failsafe to not read whole document if we dont find pagenumber correctly
            const int MaxRowsForSummary = 100;
            StringBuilder sb = new StringBuilder();
            for (int j = startIdx; j < rows.Count && j < startIdx+MaxRowsForSummary; j++)
            {
                var sumRow = currentRows[j];
                // Assume we reahed end of page and only one page for summary.
                if (sumRow.Trim() == "1" || sumRow.Trim() == "2")
                {
                    return CleanSummary(sb);
                }
                sb.AppendLine(sumRow);
            }
            return CleanSummary(sb);
        }
        string CleanSummary(StringBuilder sb)
        {
            var result = sb.ToString();
            if (string.IsNullOrEmpty(result.Trim()))
            {
                return null;
            }
            string pattern = @"\d{4}/\d{2}";

            result = Regex.Replace(result, pattern, ""); // Replace any ´1997/98 to not give ai info about years.
            if (false && !string.IsNullOrEmpty(pageFooter))
            {
               
                var parts = pageFooter.Split(' ');
                foreach(var p in parts.Where(nonEmpty => !string.IsNullOrEmpty(nonEmpty.Trim())))
                {
                    result = result.Replace(p.Trim(), ""); // Remove footer so ai cant guess from year
                }

                // Sometimes they use another space (non break space?)
                parts = pageFooter.Split('\u00A0');
                foreach (var p in parts.Where(nonEmpty => !string.IsNullOrEmpty(nonEmpty.Trim())))
                {
                    result = result.Replace(p.Trim(), ""); // Remove footer so ai cant guess from year
                }

                var splitFooter = pageFooter.Split(':');
                if (splitFooter.Length > 0)
                {
                    var subParts = splitFooter[0].Split('/');
                    if (subParts.Length == 2)
                    {
                        result = result.Replace(subParts[0].Trim(), ""); // Remove any 1986/87 as well
                    }
                }
            }
            var split = result.Split("\r\n\r\n", StringSplitOptions.RemoveEmptyEntries);
            StringBuilder newBuilder = new StringBuilder();
            foreach(var p in split)
            {
                newBuilder.AppendLine(p.Replace("\r\n", "")); // Remove weird line breaks
                newBuilder.AppendLine();
            }
            return newBuilder.ToString();
        }
        public List<DocSection> ParseSections()
        {
            var nonEmpty = currentRows.ToList();
            pageFooter = GuessFooter(nonEmpty);
            if (!string.IsNullOrEmpty(pageFooter))
            {
                Debug.WriteLine("Guessed footer " + pageFooter);
            }
            else
            {
                throw new Exception("NO FOOTER FOUND!");
                Console.WriteLine("ERROR: Couldnt find a footer guess!?");
            }
            List<DocSection> index = null;
            DocSection currSection = null;
            StringBuilder sb = new StringBuilder();
            int currPage = 0;
            for(int i = 0; i < nonEmpty.Count-1; i++)
            {
                var currRow = nonEmpty[i];
                var nextRow = nonEmpty[i + 1];
                // Check for page footer
                if (i < nonEmpty.Count - 1)
                {
                    if (IsPageFooter(currPage, nonEmpty[i], nonEmpty[i+1])) {
                        currPage++;
                        i++;
                        continue;
                    }
                }
                if (IsIndex(nonEmpty[i]))
                {
                    if (index != null)
                    {
                        Debug.WriteLine("Found multiple indices for document!?");
                        // Probably attachment
                        continue;
                    }
                    int numRowsRead;
                    index = ReadIndex(nonEmpty, i + 1, currPage, out numRowsRead);
                    i += numRowsRead;
                    continue;
                }

                if (index != null)
                {
                    var lineNumber = ParseSectionNumber(currRow);
                   if (TryReadSection(index, currRow, nextRow,  out var id,  out var rest))
                    {
                        var maybeCurrSection = index.SingleOrDefault(ds => ds.Id == id);
                        if (maybeCurrSection == null)
                        {
                            Debug.WriteLine("ERROR NO SECTION WITH ID FOUND: " + id);
                            continue;
                        }
                        if (string.IsNullOrEmpty(rest) ||rest.Length < 3)
                        {
                            if (string.IsNullOrEmpty(rest))
                            {
                                if (maybeCurrSection.Title.StartsWith(nextRow.Trim()))
                                {
                                    rest = nextRow;
                                    i++;
                                }
                                else
                                {
                                    sb.AppendLine(currRow);
                                    continue;
                                }
                            }
                            else
                            {
                                sb.AppendLine(currRow);
                                continue;
                            }
                           // Debug.WriteLine("No rest not parsing section from " + currRow);
                 
                        }
                        var restStart = rest.Substring(0, 3);
                        if (maybeCurrSection == null)
                        {
                          
                            sb.AppendLine(currRow);
                            continue;
                        }
                        if (currSection != null && maybeCurrSection.SectionNumber <= currSection.SectionNumber)
                        {
                          
                            sb.AppendLine(currRow); continue;
                        }
                        if (!maybeCurrSection.Title.StartsWith(restStart))
                        {
                            sb.AppendLine(currRow);
                            continue;
                        }
                        else
                        {
                            if (currSection != null)
                            {
                                currSection.Text = sb.ToString();
                                sb.Clear();
                              
                            }
                            
                            currSection = maybeCurrSection;
                            Debug.WriteLine("---------------Found section in document: " + id+" on row "+currRow);
                            int count = 0;
                            while (currSection.Title.Contains(nonEmpty[count+i]))
                            {
                                count++;
                            }
                            i += count; // If title is on multiple lines
                        }

                    }
                    else if (currSection != null)
                    {
                            sb.AppendLine(currRow);
                      
                    }
                }
            }

            if (sb.Length > 0 && currSection != null && string.IsNullOrEmpty(currSection.Text))
            {
                currSection.Text = sb.ToString();
            }
            return index;
        }
        bool TryReadSection(List<DocSection> sections, string row, string nextRow, out string id, out string rest)
        {
            id = null;
            rest = null;
            foreach(var s in sections.OrderByDescending(s => s.Id.Length))
            {
                if (row.StartsWith(s.Id))
                {
                    rest = row.Substring(s.Id.Length).TrimStart();
                    if (rest?.Length >= 3)
                    {
                        if (s.Title.StartsWith(rest.Substring(0, 3)))
                        {
                            id = s.Id;
                            return true;
                        }
                        rest = null;
                    }
                    else if (nextRow?.Trim()?.Length > 3)
                    {
                        if (s.Title.StartsWith(nextRow.Trim()))
                        {
                            id = s.Id;
                            return true;
                        }
                    }
                    else
                    {
                        rest = null;
                    }
                }
            }
            return false;

        }
        List<DocSection> ReadIndex(List<string> rows, int startIdx, int currPage, out int numRowsRead)
        {
            int count = startIdx;
            List<DocSection> result = new List<DocSection>();
            int counting = 0;
            numRowsRead = 0;
            double? lastParsedSection = null;
            do
            {
                var sectionNumber = rows[count];
                string prev = null;
                string next = null;
                string nextNext = null;
                string org = rows[count];
                if (rows.Count > count + 1) {
                    next = rows[count + 1];
                }
                if (rows.Count > count + 2)
                {
                    nextNext = rows[count + 2];
                }
                if (count > 0)
                {
                    prev = rows[count - 1];
                }
                if (IsPageFooter(currPage, sectionNumber, rows[count+1]))
                {
                    currPage++;
                    count += 2;
                    counting += 2;
                    continue;
                }
                if (sectionNumber == (currPage+1).ToString())
                {
                    currPage++;
                    count += 1;
                    counting += 1;
                    continue;
                }
                var restName = TryParseSectionId(sectionNumber, out string parsedId);
                string name = null;
                if (!string.IsNullOrEmpty(parsedId))
                {
                    sectionNumber = parsedId;
                    if (!string.IsNullOrEmpty(restName))
                    {
                        name = restName;
                    }
                    if (IsPageFooter(currPage, rows[count+1], rows[count + 2]))
                    {
                        currPage++;
                        count += 2;
                        counting += 2;
                    }
                }
                string page = null;
                int curr = -1;
                if (!string.IsNullOrEmpty(name))
                {
                    count--;
                    counting--;
                }
                else
                {
                    name = rows[count + 1];
                }
                page = rows[count + 2].Trim();
                curr = count + 2;
                string orgName = name.Trim();

               
                int start = curr;
                int pageNumber = 0;
                const int MaxRowsForIndexName = 6; // Failsafe dont read all document
                while(!int.TryParse(page, out pageNumber))
                {
                    name += page;
                    curr++;
                    count++;
                    counting++;
                    if (curr >= rows.Count || MaxRowsForIndexName < (curr-start))
                    {
                        break;
                    }
                    page = rows[curr];
                }
                if (pageNumber <= 0)
                {
                    break;
                }
                lastParsedSection = ParseSectionNumber(sectionNumber);

                if (lastParsedSection.HasValue && int.TryParse(page, out var _))
                {
                    var id = sectionNumber.Trim();
                    var existing = result.SingleOrDefault(ds => ds.Id == id);
                    if (existing != null)
                    {
                        Debug.WriteLine("ALREADY EXIST ONE WITH " + id);
                        throw new Exception("SEctionId must be unique " + id);
                    }
                    var ds = new DocSection();
                    ds.Title = name.Trim().Trim('.').Trim();
                    ds.Id = id;
                    ds.SectionNumber = lastParsedSection.Value;
                    ds.Page = pageNumber;
                    result.Add(ds);
                    count += 3;
                    counting += 3;
                }
                else
                {
                    count += 3;
                    counting += 3;
                    break;
                }
               
            } while (rows.Count > count + 2 && lastParsedSection.HasValue);
            numRowsRead = counting;

            return result;
        }


        public string TryParseSectionId(string row, out string id)
        {
            StringBuilder sb = new StringBuilder();
            id = null;
            foreach(var c in row)
            {
                if (char.IsDigit(c) || c == '.')
                {
                    sb.Append(c);
                }
                else
                {
                    break;
                }
            }
            if (sb.Length == 0)
                return null;
            id = sb.ToString();
            if (id.EndsWith("."))
            {
                id = null;
                return null;
            }    
            return row.Substring(sb.Length).Trim();
        }
        public static int? ParseMainSectionNumber(string text)
        {
            var parts = text.Split(' ');
            if (parts.Length == 0) return null;
            if (int.TryParse(parts[0], out var wholeSectionNumber))
                return wholeSectionNumber;
            return null;
        }

        public const int MaxNumbersInSectionPart = 3;
        public static double? ParseSectionNumber(string text)
        {
            var parts = text.Split('.');
            double val = 0;
            for (int i = 0; i < parts.Length; i++)
            {
                if (!int.TryParse(parts[i], out var number))
                {
                    return null; // Invalid
                }
                double log = Math.Floor(Math.Log10(number));
                var div = Math.Pow(10, i * MaxNumbersInSectionPart);
                val += number / div;
            }
            return val != 0 ? val : null;
        }
        bool IsIndex(string row)
        {
            return row.Contains("Innehållsförteckning");
        }
        bool IsPageFooter(int currPage, string pageText, string footerText)
        {
            if (int.TryParse(pageText.Trim(), out var page) && page == currPage + 1)
            {
                return footerText.Trim() == pageFooter;
            }
            return false;
        }

    }
}
