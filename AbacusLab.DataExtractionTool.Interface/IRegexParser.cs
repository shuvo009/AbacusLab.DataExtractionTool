using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbacusLab.DataExtractionTool.Interface
{
    public interface IRegexParser
    {
        string Parse(string text);
    }
}
