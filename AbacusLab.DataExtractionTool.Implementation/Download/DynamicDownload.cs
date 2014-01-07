using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface.Download;
using Newtonsoft.Json.Linq;

namespace AbacusLab.DataExtractionTool.Implementation.Download
{
    public class DynamicDownload : IDynamicDownload
    {
        public async Task<dynamic> Download(string url)
        {
            var webRequest = WebRequest.Create(url);
            webRequest.Timeout = 20000;
            webRequest.Method = "GET";
            var responce = await webRequest.GetResponseAsync();

            using (var responceStream = responce.GetResponseStream())
            {
                if (responceStream != null)
                {
                    var reader = new StreamReader(responceStream);
                    var jsonText = await reader.ReadToEndAsync();
                    return JObject.Parse(jsonText);
                }
            }
            return null;
        }
    }
}
