using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface.Base;

namespace AbacusLab.DataExtractionTool.Interface.Download
{
    public interface IDynamicDownload
    {
        Task<dynamic> Download(string url);
    }
}
