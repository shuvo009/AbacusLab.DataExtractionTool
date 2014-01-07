using System.Threading.Tasks;

namespace AbacusLab.DataExtractionTool.Interface.Download
{
    public interface IStringDownload
    {
         Task<string> Download(string url, string saveFilePath = "");
    }
}