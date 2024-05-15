using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Riksdagen
{
    internal class FileSelector
    {
        public void CopyFiles<T>(string inputDir, string outputDir, Func<T, bool> filterFunc, Func<string,T> creatorFunc = null)
        {
            if (creatorFunc == null)
            {
                creatorFunc = (s) => JsonSerializer.Deserialize<T>(s);
            }

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            int numTotal = 0;
            int numCopied = 0;
            int numFailed = 0;
            foreach(var f in Directory.GetFiles(inputDir))
            {
                numTotal++;
                try
                {
                    var jsonText = File.ReadAllText(f);
                    var obj = creatorFunc(jsonText);
                    if (filterFunc(obj))
                    {
                        var outputName = outputDir + Path.GetFileName(f);
                        File.Copy(f, outputName, true);
                        numCopied++;
                    }
                }
                catch(Exception ex)
                {
                    numFailed++;
                    Console.WriteLine("ERROR processing file: " + f);
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
