using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbacusLab.DataExtractionTool.Interface.Html
{
    public interface IHtmlParser
    {
        Task<List<string>> ExtractHrefFromTable(string html, string className, int columnPosition = 0, int tablePosition = 0, string urlFormat = "", bool isAttribute = true);
        void ExtractDataFromGoogleMapPage(string firstPageHtml);
    }
}
