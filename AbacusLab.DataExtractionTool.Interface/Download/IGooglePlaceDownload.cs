using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface.Base;

namespace AbacusLab.DataExtractionTool.Interface.Download
{
    public interface IGooglePlaceDownload : IDownloadBase
    {
        Task<bool> DownloadPlaceInfo(string searchText, string saveFilePath);
        void ReadResumeFile(string fileName);
        ObservableCollection<string> ResumeFileList { get; set; }
    }
}
