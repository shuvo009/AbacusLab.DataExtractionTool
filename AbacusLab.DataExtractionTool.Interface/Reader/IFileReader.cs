using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbacusLab.DataExtractionTool.Interface
{
    public interface IFileReader
    {
         IEnumerable<string> ReadFromFile(string filePath, int columnNumber = 1);
        double TotalLine(string filePath);
    }
}
