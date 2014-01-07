using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface;

namespace AbacusLab.DataExtractionTool.Implementation.File
{
    public class ReadFromText : IFileReader
    {
        public IEnumerable<string> ReadFromFile(string filePath, int columnNumber = 1)
        {
            using (var sr = new StreamReader(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        public double TotalLine(string filePath)
        {
            return 0;
        }
    }
}
