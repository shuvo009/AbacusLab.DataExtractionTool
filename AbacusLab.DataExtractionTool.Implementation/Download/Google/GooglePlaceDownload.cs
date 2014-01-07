using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Google;
using AbacusLab.DataExtractionTool.Implementation.Base;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface.Download;
using Microsoft.Practices.Unity;
using Newtonsoft.Json.Linq;
using IO = System.IO;
namespace AbacusLab.DataExtractionTool.Implementation.Download.Google
{
    public class GooglePlaceDownload : DownloadBase, IGooglePlaceDownload
    {
        private string _currentKey;
        private readonly IDynamicDownload _dynamicDownload;
        private readonly GoogleUrlMaker _urlMaker;
        private string _savePath;
        private string _searchText;
        public ObservableCollection<string> ResumeFileList { get; set; }
        public GooglePlaceDownload()
        {
            ProgressMaxValue = 60;
            ResumeFileList = new ObservableCollection<string>();
            _currentKey = ApiKeys.Keys.GetApiKey();
            _dynamicDownload = DependenceResolver.Resolver.Container.Resolve<IDynamicDownload>();
            _urlMaker = new GoogleUrlMaker();
            LoadResumeFiles();
        }

        public async Task<bool> DownloadPlaceInfo(string searchText, string saveFilePath)
        {
            _savePath = saveFilePath;
            _searchText = searchText;
            AddTitles();
            try
            {
                IsIndeterminate = true;
                var referenceList = await SearchResultDownload(searchText);
                var totalItem = referenceList.Count;
                IsIndeterminate = false;
                for (int i = 0; i < totalItem; i++)
                {
                    string reference = referenceList[i];
                    var isSuccess = await DownloadDetails(reference);
                    if (!isSuccess)
                    {
                        ResumeDownload(referenceList, i, totalItem);
                        MessageService.ShowMessages("OVER QUERY LIMIT\nDownload Resume. Please Try Again Tomorrow.");
                        return false;
                    }

                    ProgressComplete++;
                }
                MessageService.ShowMessages("Work Complete !");
            }
            catch (Exception ex)
            {
                MessageService.ShowMessages(ex.Message);
            }
            finally
            {
                IsEnable = true;
                IsIndeterminate = false;
            }
            return true;
        }

        public async void ReadResumeFile(string fileName)
        {
            var filePath = string.Format(@"Resume\{0}", fileName);
            var text = IO.File.ReadAllText(filePath);
            dynamic resumeData = JObject.Parse(text);
            int start = resumeData.start;
            _savePath = resumeData.savefilepath;
            var totalItem = CountItem(resumeData.references as IEnumerable);
            ProgressMaxValue = totalItem + Convert.ToDouble(start);
            ProgressComplete = start;
            _currentKey = ApiKeys.Keys.GetApiKey();
            for (var i = 0; i < totalItem; i++)
            {
                string reference = resumeData.references[i];
                var isSuccess = await DownloadDetails(reference);
                if (!isSuccess)
                {
                    MessageService.ShowMessages("OVER QUERY LIMIT\nDownload Resume. Please Try Again Tomorrow.");
                    return;
                }
                ProgressComplete++;
            }
            IO.File.Delete(filePath);
            LoadResumeFiles();
            MessageService.ShowMessages("Download Complete.");
        }

        #region Private
        private async Task<List<string>> SearchResultDownload(string searchText, string nextPageTocken = "", int resultCount = 0)
        {
            var referenceList = new List<string>();
        fullTextSearchTryAgain:
            var fullTextSearchUrl = _urlMaker.PrepareUrl(searchText, true, _currentKey, nextPageTocken);
            var fullTextSearchResult = await _dynamicDownload.Download(fullTextSearchUrl);
            string status = fullTextSearchResult.status;
            if (status.Equals("OVER_QUERY_LIMIT"))
            {
                _currentKey = ApiKeys.Keys.GetApiKey();
                if (string.IsNullOrEmpty(_currentKey))
                {
                    MessageService.ShowMessages("OVER QUERY LIMIT");
                    throw new Exception("OVER QUERY LIMIT");
                }
                goto fullTextSearchTryAgain;
            }
            nextPageTocken = fullTextSearchResult.next_page_token;
            foreach (var result in fullTextSearchResult.results)
            {
                string reference = result.reference;
                referenceList.Add(reference);
                resultCount++;
            }
            if (string.IsNullOrEmpty(nextPageTocken))
                return referenceList;
            await Task.Delay(15000);
            referenceList.AddRange(await SearchResultDownload(searchText, nextPageTocken, resultCount));
            return referenceList;
        }

        private async Task<bool> DownloadDetails(string reference)
        {
        detailSearchTryAgain:
            var detailsUrl = _urlMaker.PrepareUrl(reference, false, _currentKey);
            var detailSearchResult = await _dynamicDownload.Download(detailsUrl);
            string status = detailSearchResult.status;
            if (status.Equals("OVER_QUERY_LIMIT"))
            {
                _currentKey = ApiKeys.Keys.GetApiKey();
                if (string.IsNullOrEmpty(_currentKey))
                {
                    return false;
                }
                goto detailSearchTryAgain;
            }
            var placeInfo = new PlaceInfo
            {
                Name = detailSearchResult.result.name,
                Lat = detailSearchResult.result.geometry.location.lat,
                Lng = detailSearchResult.result.geometry.location.lng,
                FormattedAddress = detailSearchResult.result.formatted_address,
                WebSite = detailSearchResult.result.website,
                FormattedPhoneNumber = detailSearchResult.result.formatted_phone_number
            };
            return await ExcelWriterService.SaveInFile(placeInfo, _savePath);
        }

        private void ResumeDownload(dynamic result, int resumeStart, double length)
        {
            var resumeTextBuilder = new StringBuilder();
            resumeTextBuilder.AppendLine("{");
            resumeTextBuilder.AppendLine(string.Format(@"""start"" : {0},", resumeStart));
            resumeTextBuilder.AppendLine(string.Format(@"""savefilepath"" : ""{0}"",", _savePath.Replace("\\", "/")));
            resumeTextBuilder.AppendLine(@"""references"" : [ ");
            for (int i = resumeStart; i < length; i++)
            {
                resumeTextBuilder.AppendLine(String.Format(@"""{0}"",", result[i]));
            }
            resumeTextBuilder.AppendLine(@" ]");
            resumeTextBuilder.AppendLine("}");
            if (!Directory.Exists(@"Resume"))
                Directory.CreateDirectory(@"Resume");
            IO.File.WriteAllText(String.Format(@"Resume\Resume {0}.resumegmap", _searchText), resumeTextBuilder.ToString());
            LoadResumeFiles();
        }

        private void LoadResumeFiles()
        {
            ResumeFileList.Clear();
            var dirInfo = new DirectoryInfo(@"Resume");
            if (!dirInfo.Exists) return;
            dirInfo.GetFiles().Where(x => x.Extension.Equals(".resumegmap"))
                .ToList()
                .ForEach(x => ResumeFileList.Add(Path.GetFileName(x.FullName)));

        }

        private double CountItem(IEnumerable result)
        {
            double count = 0;
            var enumerator = result.GetEnumerator();
            while (enumerator.MoveNext())
            {
                count++;
            }
            return count;
        }

        private void AddTitles()
        {
            var placeInfo = new PlaceInfo
            {
                Name = "Name",
                Lat = "Lat",
                Lng = "Lng",
                FormattedAddress = "Formatted Address",
                WebSite = "WebSite",
                FormattedPhoneNumber = "Formatted Phone Number"
            };
            ExcelWriterService.SaveInFile(placeInfo, _savePath);
        }

        #endregion

    }
}
