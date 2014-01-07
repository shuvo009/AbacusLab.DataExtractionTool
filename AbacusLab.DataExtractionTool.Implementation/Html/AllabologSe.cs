using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Html;
using AbacusLab.DataExtractionTool.Entitys.Interface;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using AbacusLab.DataExtractionTool.Interface.Html;
using HtmlAgilityPack;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.Implementation.Html
{
    public class AllabologSe : IHtmlParserExt
    {
        public async Task<IHtmlEntity> ParseHtml(HtmlParserParameterList htmlParserParameter)
        {
            return await Task.Run(() =>
            {
                var htmlDoc = new HtmlDocument();
                var allabolagSeEntity = new AllabolagSeEntity();
                htmlDoc.LoadHtml(htmlParserParameter.Html);
                allabolagSeEntity.CorporateRegistration = htmlParserParameter.AdditionalParameterOne;
                allabolagSeEntity.CompanyName = htmlDoc.GetElementbyId("printTitle").InnerText;
                var reportContentTable = htmlDoc.GetElementbyId("printContent").FirstChild.NextSibling.CloneNode(true);
                var taleRows = reportContentTable.Descendants("table").ToList().First().Descendants("tr").ToList();

                var addressInc = 0;
                StringBuilder addressBuilder = new StringBuilder();
                foreach (var htmlNode in taleRows)
                {
                    var interText = htmlNode.InnerText.Trim();
                    if (interText.Contains("Telefon"))
                    {
                        allabolagSeEntity.CompanyPhone = interText.Split(':')[1];
                    }
                    else if (interText.Contains("ADRESS") || addressInc > 0)
                    {
                        if (addressInc > 0)
                        {
                            addressBuilder.Append("___" + interText.Replace("&nbsp;", ""));
                        }
                        addressInc++;
                        if (addressInc == 4)
                            break;
                    }
                }
                var address = addressBuilder.ToString().Split(new[] { "___" }, StringSplitOptions.None);
                allabolagSeEntity.CompanyAddress = address[1];
                allabolagSeEntity.CompanyCounty = address[3];
                var zipAndCity = address[2].Split(' ');
                allabolagSeEntity.CompanyCity = zipAndCity.Last();
                allabolagSeEntity.CompanyZipCode = zipAndCity.Count() > 2
                    ? string.Format("{0} {1}", zipAndCity[0], zipAndCity[1])
                    : zipAndCity.First();

                var bokslutRows = reportContentTable.Descendants("tr").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value.Equals("bgLightPink"));
                foreach (var bokslutRow in bokslutRows)
                {
                    var innerText = bokslutRow.InnerText.Trim();
                    if (innerText.Contains("Antal anst"))
                    {
                        var antal = bokslutRow.InnerText.Trim().Split(new[] { "            " }, StringSplitOptions.None).Where(x => x.Trim() != "&nbsp;").ToList();
                        var antalCount = antal.Count();
                        if (antalCount == 1)
                            allabolagSeEntity.NumberOfEmployees = antal[0].Split(':')[1].Trim();
                        if (antal.Count() > 2)
                            allabolagSeEntity.NumberOfEmployees = antal[2].Trim();
                    }
                    else if (innerText.Contains("Oms"))
                    {
                        var omsättning = bokslutRow.InnerText.Trim().Split(new[] { "            " }, StringSplitOptions.None).Where(x => x.Trim() != "&nbsp;").ToList();
                        var omsCount = omsättning.Count();
                        if (omsCount == 1)
                            allabolagSeEntity.AnnualTurnover = omsättning[0].Split(':')[1].Trim();

                        if (omsättning.Count() > 2)
                            allabolagSeEntity.AnnualTurnover = omsättning[2].Trim();
                        break;
                    }
                }

                var htmlDownload = Dependence.DependenceResolver.Resolver.Container.Resolve<IStringDownload>("Html");
                htmlDoc = new HtmlDocument();
                var htmlFilePath = htmlDownload.Download(string.Format("http://www.allabolag.se/{0}/verksamhet", allabolagSeEntity.CorporateRegistration.Replace("-", ""))).Result;
                htmlDoc.LoadHtml(System.IO.File.ReadAllText(htmlFilePath));
                reportContentTable = htmlDoc.GetElementbyId("printContent").FirstChild.NextSibling.CloneNode(true);

                var sNINode = reportContentTable.Descendants("tr").ToList()
                    .First(x => x.InnerText.Equals("&nbsp;SVENSK NÄRINGSGRENSINDELNING - SNI&nbsp;"))
                    .NextSibling.NextSibling;

                allabolagSeEntity.CompanySIC = sNINode.InnerText.Trim().Replace("&nbsp;", "");
                return allabolagSeEntity;
            });
        }
    }
}
