using System.Text.RegularExpressions;
using AbacusLab.DataExtractionTool.Implementation.Data;
using AbacusLab.DataExtractionTool.Interface;

namespace AbacusLab.DataExtractionTool.Implementation.RegexParser
{
    public class ParserWebSite : IRegexParser
    {
        public string Parse(string text)
        {
            foreach (var webReg in RegexCollection.WebSiteRegex)
            {
                var regex = new Regex(webReg, RegexOptions.IgnoreCase);

                var match = regex.Match(text);
                if (match.Success)
                {
                    return match.Value;
                }
            }
            return string.Empty;
        }
    }
}