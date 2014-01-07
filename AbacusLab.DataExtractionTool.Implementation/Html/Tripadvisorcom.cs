using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Html;
using AbacusLab.DataExtractionTool.Entitys.Interface;
using AbacusLab.DataExtractionTool.Interface.Html;
using ClosedXML.Excel;
using HtmlAgilityPack;

namespace AbacusLab.DataExtractionTool.Implementation.Html
{
    public class Tripadvisorcom : IHtmlParserExt
    {
        public async Task<IHtmlEntity> ParseHtml(HtmlParserParameterList htmlParserParameter)
        {
            return await Task.Run(() =>
            {
                var htmlDoc = new HtmlDocument();
                var tripadvisorcomEntity = new TripadvisorcomEntity();
                htmlDoc.LoadHtml(htmlParserParameter.Html);
                var img = htmlDoc.DocumentNode.Descendants("img").Where(d =>
                                           d.Attributes.Contains("id")
                                           && d.Attributes["id"].Value.Contains("lazyload")
                                            && d.Attributes.Contains("src")
                                           && d.Attributes["src"].Value.Contains("http://dev.virtualearth.net")).ToList();
                var interHtml = img.First().OuterHtml;
                var stratIndex = interHtml.LastIndexOf("pp=", System.StringComparison.Ordinal) + 3;
                var endIndex = interHtml.LastIndexOf(";59", System.StringComparison.Ordinal);
                var stringLength = endIndex - stratIndex;
                var latLon = interHtml.Substring(stratIndex, stringLength).Split(',');
                tripadvisorcomEntity.Lat = latLon[0];
                tripadvisorcomEntity.Lon = latLon[1];
                return tripadvisorcomEntity;
            });
        }
    }
}
