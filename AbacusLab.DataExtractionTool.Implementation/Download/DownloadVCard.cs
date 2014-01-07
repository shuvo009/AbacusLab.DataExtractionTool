using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using FileIo = System.IO;
namespace AbacusLab.DataExtractionTool.Implementation.Download
{
    public class DownloadVCard : IStringDownload
    {
        public async Task<string> Download(string url, string saveFilePath = "")
        {
            try
            {
                return await Task.Run(() =>
                {
                    if (String.IsNullOrEmpty(saveFilePath))
                        saveFilePath = Path.Combine(Path.GetTempPath(), "AbacusLabVcard.vcf");
                    Byte[] bytes;
                    var webRequest = (HttpWebRequest)WebRequest.Create(url);
                    webRequest.KeepAlive = true;
                    webRequest.ProtocolVersion = HttpVersion.Version10;
                    webRequest.ServicePoint.ConnectionLimit = 24;
                    webRequest.Headers.Add("UserAgent", "Pentia; MSI");
                    using (var webResponse = webRequest.GetResponse())
                    {
                        using (var stream = webResponse.GetResponseStream())
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                var buffer = new Byte[0x1000];
                                Int32 bytesRead;
                                while (stream != null && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    memoryStream.Write(buffer, 0, bytesRead);
                                }
                                bytes = memoryStream.ToArray();
                            }
                        }
                    }
                    using (var fileStream = FileIo.File.Create(saveFilePath, (int)bytes.Length))
                    {
                        fileStream.Write(bytes, 0, bytes.Length);
                    }
                    return saveFilePath;
                });
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
