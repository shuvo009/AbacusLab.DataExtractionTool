using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbacusLab.DataExtractionTool.Interface
{
    public interface IHtmlParser
    {
        Task<List<string>> ExtractHrefFromTable(string html, string className, int columnPosition = 0, int tablePosition = 0, string urlFormat = "", bool isAttribute = true);
    }
}
