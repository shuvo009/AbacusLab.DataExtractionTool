using System;
using System.Net;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using FileIo = System.IO;

namespace AbacusLab.DataExtractionTool.Implementation.Download
{
    public class DownloadHtml : IStringDownload
    {
        public async Task<string> Download(string url, string saveFilePath = "")
        {
            if (String.IsNullOrEmpty(saveFilePath))
                saveFilePath = FileIo.Path.Combine(FileIo.Path.GetTempPath(), "AbacusLabTempTextFile.txt");

            var webClient = new WebClient();
            string htmlString = await webClient.DownloadStringTaskAsync(new Uri(url));
            FileIo.File.WriteAllText(saveFilePath, htmlString);
            return saveFilePath;
        }
    }
}