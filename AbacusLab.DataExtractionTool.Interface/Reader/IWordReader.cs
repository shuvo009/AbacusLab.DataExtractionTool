using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Word;

namespace AbacusLab.DataExtractionTool.Interface.Reader
{
    public interface IWordReader
    {
        IEnumerable<WordEntity> ReadFromFile(string filePath);
        double TotalLine(string filePath);
    }
}
