using System.Text.RegularExpressions;
using AbacusLab.DataExtractionTool.Implementation.Data;
using AbacusLab.DataExtractionTool.Interface;

namespace AbacusLab.DataExtractionTool.Implementation.RegexParser
{
    public class ParseEmail : IRegexParser
    {
        public string Parse(string text)
        {
            foreach (var emailReg in RegexCollection.EmailRegex)
            {
                var regex = new Regex(emailReg, RegexOptions.IgnoreCase);

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
