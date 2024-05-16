using Riksdagen.Import.ExportedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Riksdagen.Import
{
    public class TagHelper
    {
        public const string regeringsName = "regeringsnamn";
        public const string leftRightCenter = "leftrightcenter";
        public static void TagByRegering<T>(IEnumerable<T> models)
               where T : ITaggable, IDatable
        {
            Dictionary<DateTime, string> endDates = new Dictionary<DateTime, string>();

            endDates.Add(new DateTime(1976, 10, 8), "Palme 1");
            endDates.Add(new DateTime(1978, 10, 18), "Fälldin 1");
            endDates.Add(new DateTime(1979, 10, 12), "Ullsten");
            endDates.Add(new DateTime(1981, 5, 22), "Fälldin 2");
            endDates.Add(new DateTime(1982, 10, 8), "Fälldin 3");
            endDates.Add(new DateTime(1986, 3, 13), "Palme 2");
            endDates.Add(new DateTime(1990, 2, 27), "Carlsson 1");
            endDates.Add(new DateTime(1991, 10, 4), "Carlsson 2");
            endDates.Add(new DateTime(1994, 10, 7), "Bildt");
            endDates.Add(new DateTime(1996, 3, 22), "Carlsson 3");
            endDates.Add(new DateTime(2006, 10, 6), "Persson 1");
            endDates.Add(new DateTime(2014, 10, 3), "Reinfeldt");
            endDates.Add(new DateTime(2019, 1, 21), "Löfven 1");
            endDates.Add(new DateTime(2021, 7, 9), "Löfven 2");
            endDates.Add(new DateTime(2021, 11, 30), "Löfven 3");
            endDates.Add(new DateTime(2022, 10, 18), "Andersson");
            endDates.Add(DateTime.MaxValue, "Kristersson");

            List<string> leftNames = new List<string>
            {
                "Palme",
                "Carlsson",
                "Persson",
                "Andersson",
                "Löfven",
            };

            List<string> centerNames = new List<string>
            {
                "Fälldin",
                "Ullsten",
            };

            List<string> rightNames = new List<string>
            {
                "Bildt",
                "Reinfeldt",
                "Kristersson",
            };
            TagByDate(models, regeringsName, endDates);
            Action<T> tagFunc = (t) =>
            {
                if (leftNames.Any(name => t.GetTag(regeringsName).StartsWith(name)))
                {
                    t.ApplyTag(leftRightCenter, "left");
                }
                else if (centerNames.Any(name => t.GetTag(regeringsName).StartsWith(name)))
                {
                    t.ApplyTag(leftRightCenter, "center");
                }
                else if (rightNames.Any(name => t.GetTag(regeringsName).StartsWith(name)))
                {
                    t.ApplyTag(leftRightCenter, "right");
                }
                else
                {
                    throw new Exception("Should have tagged all models with known names "+ t.GetTag(regeringsName));
                }
            };
            ApplyActionFunc(models, tagFunc);
        }
            public static void TagByDate<T>(IEnumerable<T> models, string tagType, Dictionary<DateTime, string> tagsPerEndDate)
             where T : ITaggable, IDatable
        {
            DateTime prevDate = DateTime.MinValue;
            foreach(var kvp in tagsPerEndDate.OrderBy(pair => pair.Key))
            {
                var currModels = models.Where(m => m.DokDate.HasValue && m.DokDate.Value > prevDate && m.DokDate.Value <= kvp.Key)
                    .ToList();
                ApplyTag(currModels, tagType, kvp.Value);
                prevDate = kvp.Key;
            }
        }
        public static void ApplyTag<T>(IEnumerable<T> models, string tagType, string tagValue)
            where T : ITaggable
        {
            ApplyActionFunc(models, m => m.ApplyTag(tagType, tagValue));
        }

        public static void ApplyActionFunc<T>(IEnumerable<T> models, Action<T> tagAction)
        {
            foreach (var m in models)
            {
                tagAction(m);
            }
        }
    }
}
