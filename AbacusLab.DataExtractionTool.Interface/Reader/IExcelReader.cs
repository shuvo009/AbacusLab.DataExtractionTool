using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbacusLab.DataExtractionTool.Interface
{
    public interface IExcelReader
    {
        IEnumerable<List<string>> ReadExcel(string filePath, int columnNumber = 1);
        double TotalRows(string filePath);
    }
}
