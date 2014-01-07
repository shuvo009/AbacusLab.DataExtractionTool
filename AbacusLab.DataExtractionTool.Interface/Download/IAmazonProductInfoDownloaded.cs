using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface.Base;

namespace AbacusLab.DataExtractionTool.Interface.Download
{
    public interface IAmazonProductInfoDownloaded : IDownloadBase
    {
        Task<bool> DownloadProductByLookup(string isbnOrAsin, bool searchTypeIsIsbn, string saveFilePath);
        Task<bool> DownloadProductBySearch(string searchText, string searchCategory, string saveFilePath);
    }
}
