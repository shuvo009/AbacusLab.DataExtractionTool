using System.Collections.Generic;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Html;
using AbacusLab.DataExtractionTool.Entitys.Interface;

namespace AbacusLab.DataExtractionTool.Interface.Html
{
    public interface IHtmlParserExt
    {
        Task<IHtmlEntity> ParseHtml(HtmlParserParameterList htmlParserParameter);
    }
}
