using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Html;
using AbacusLab.DataExtractionTool.Implementation.Download;
using AbacusLab.DataExtractionTool.Implementation.File;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using AbacusLab.DataExtractionTool.Interface.Html;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using HtmlAgilityPack;

namespace AbacusLab.DataExtractionTool.Implementation.Html
{
    public class HtmlParser : IHtmlParser
    {
        public Task<List<string>> ExtractHrefFromTable(string html, string className, int columnPosition = 0, int tablePosition = 0, string urlFormat = "", bool isAttribute = true)
        {
            return Task.Run(() =>
            {
                var hrefs = new List<string>();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);
                var tables = htmlDoc.DocumentNode.Descendants("table").Where(d =>
                                           d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals(className)).ToList();
                if (tables.Any() && tables.Count() >= tablePosition)
                {
                    var tableBody = tables[tablePosition];
                    if (tables[tablePosition].ChildNodes.Any(x => x.Name == "tbody"))
                    {
                        tableBody = tables[tablePosition].ChildNodes.Last(x => x.Name == "tbody");
                    }
                    foreach (var tableRow in tableBody.ChildNodes.Where(x => x.Name == "tr"))
                    {
                        var link = String.Empty;
                        if (columnPosition > 0)
                        {
                            var columns = tableRow.ChildNodes.Where(x => x.Name == "td").ToList();
                            if (columns.Count >= columnPosition)
                            {
                                if (isAttribute)
                                    link = columns[columnPosition].ChildNodes.First().GetAttributeValue("href", " ");
                                else
                                    link = columns[columnPosition].ChildNodes.First().InnerText.Replace(":", "").Trim();
                            }
                        }
                        else
                        {
                            var onClickText = tableRow.GetAttributeValue("onclick", " ");
                            if (!String.IsNullOrEmpty(onClickText))
                            {
                                var firstCote = onClickText.IndexOf("'", System.StringComparison.Ordinal) + 1;
                                var lastCote = onClickText.LastIndexOf("'", System.StringComparison.Ordinal);
                                var idLength = lastCote - firstCote;
                                link = onClickText.Substring(firstCote, idLength);
                            }
                        }

                        if (string.IsNullOrEmpty(link) && isAttribute) continue;
                        if (!String.IsNullOrEmpty(urlFormat))
                            link = String.Format(urlFormat, link);
                        hrefs.Add((link));
                    }
                }
                return hrefs;
            });
        }


        public List<string> ExtractIdFromTable(string html, string className, int columnPosition = 0, int tablePosition = 0, string urlFormat = "")
        {
            var hrefs = new List<string>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var tables = htmlDoc.DocumentNode.Descendants("table").Where(d =>
                                       d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals(className)).ToList();
            if (tables.Any() && tables.Count() >= tablePosition)
            {
                var tableBody = tables[tablePosition].ChildNodes.Last(x => x.Name == "tbody");
                foreach (var tableRow in tableBody.ChildNodes)
                {
                    var columns = tableRow.ChildNodes.Where(x => x.Name == "td").ToList();
                    if (columns.Count >= columnPosition)
                    {
                        var link = columns[columnPosition].ChildNodes.First().GetAttributeValue("href", "Non");
                        if (!String.IsNullOrEmpty(urlFormat))
                            link = String.Format(urlFormat, link);
                        hrefs.Add((link));
                    }
                }
            }
            return hrefs;
        }


        public void ExtractDataFromGoogleMapPage(string pageUrl)
        {
            IStringDownload htmlDownload = new DownloadHtml();

            var fileUrl = htmlDownload.Download(pageUrl).Result;


            var pageHtml = System.IO.File.ReadAllText(fileUrl);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageHtml);
            var allDiv = htmlDoc.DocumentNode.Descendants("div").Where(d =>
                                           d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("text vcard indent block")).ToList();

            IFileWriter excelWrite = new ExcelWriter();
            foreach (var div in allDiv)
            {
                var mapInfo = new MapsGooglePageInfo();

                var titleNode = div.Descendants("span").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("pp-place-title"));
                mapInfo.Title = titleNode != null ? titleNode.InnerText : string.Empty;

                var addressNode = div.Descendants("span").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("pp-headline-item pp-headline-address"));
                mapInfo.Address = addressNode != null ? addressNode.InnerText : string.Empty;

                var telephoneNode = div.Descendants("span").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("telephone"));
                mapInfo.Phone = telephoneNode != null ? telephoneNode.InnerText : string.Empty;

                var webSiteNode = div.Descendants("span").FirstOrDefault(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("pp-headline-item pp-headline-authority-page"));
                mapInfo.WebSite = webSiteNode != null ? webSiteNode.InnerText : string.Empty;

                var isSucces = excelWrite.SaveInFile(mapInfo, @"E:\googleMapOutput.xlsx").Result;
            }

            var navDiv = htmlDoc.DocumentNode.Descendants("div").FirstOrDefault(d => d.Attributes["id"] != null && d.Attributes["id"].Value.Equals("navbar"));
            if (navDiv == null) return;
            var table = navDiv.Descendants("table").FirstOrDefault();
            if (table == null) return;
            var nextLink = table.Descendants("td").FirstOrDefault(d => d.Attributes["align"] == null && d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("b"));
            if (nextLink == null) return;
            var getAElement = nextLink.Descendants("a").FirstOrDefault();
            if (getAElement == null) return;
            var nextPageLink = getAElement.GetAttributeValue("href", " ");
            var actualLink = String.Format("https://maps.google.co.uk{0}", nextPageLink);
            ExtractDataFromGoogleMapPage(actualLink);
        }
    }
}
