using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Html;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Implementation.File;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using AbacusLab.DataExtractionTool.Interface.Html;
using Xunit;
using Microsoft.Practices.Unity;
namespace AbacusLab.DataExtractionTool.UnitTest
{
    public class HtmlParseTest
    {
        [Fact]
        public void GetLinksFromATable()
        {
            var htmlParser = DependenceResolver.Resolver.Container.Resolve<IHtmlParser>();
            var htmldoc = File.ReadAllText(@"H:\html.txt");
            var links = htmlParser.ExtractHrefFromTable(htmldoc, "MainContent", 4, 1, @"http://idaman2.kpkt.gov.my:8888/idv5/98_eHome/{0}").Result;

            Assert.NotEqual(links.Count, 0);
            File.WriteAllLines(@"H:\out.txt", links);
        }

        [Fact]
        public void GetLinksFromSecondPageTable()
        {
            var htmlParser = DependenceResolver.Resolver.Container.Resolve<IHtmlParser>();
            var htmldoc = File.ReadAllText(@"H:\SecondPage.txt");
            var links = htmlParser.ExtractHrefFromTable(htmldoc, "MainContent", tablePosition: 1, urlFormat: @"http://idaman2.kpkt.gov.my:8888/idv5/98_eHome/template/pengarahDetails.cfm?pmju_Kod=9475&proj_Kod_Fasa=1&rekid={0}").Result;

            Assert.NotEqual(links.Count, 0);
            File.WriteAllLines(@"H:\Second.txt", links);
        }

        [Fact]
        public void GetInformationFromTable()
        {
            var htmlParser = DependenceResolver.Resolver.Container.Resolve<IHtmlParser>();
            var htmldoc = File.ReadAllText(@"H:\info.txt");
            var links = htmlParser.ExtractHrefFromTable(htmldoc, "MainContent", columnPosition: 1, tablePosition: 0, isAttribute: false).Result;

            Assert.NotEqual(links.Count, 0);
            File.WriteAllLines(@"H:\infoOut.txt", links);
        }

        [Fact]
        public void GoogleMapTest()
        {

            IFileWriter excelWrite = new ExcelWriter();
            var mapInfo = new MapsGooglePageInfo
            {
                Address = "Address",
                Phone = "Phone",
                Title = "Title",
                WebSite = "WebSite"
            };

            var isSuccess = excelWrite.SaveInFile(mapInfo, @"E:\googleMapOutput.xlsx").Result;

            var htmlParser = DependenceResolver.Resolver.Container.Resolve<IHtmlParser>();
            //     var htmldoc = File.ReadAllText(@"E:\googleMapSearch.txt");
            htmlParser.ExtractDataFromGoogleMapPage(@"https://maps.google.co.uk/maps?q=mobile%20application%20developers%20in%20uk&espv=210&es_sm=93&um=1&ie=UTF-8&hl=en&sa=N&tab=wl");
        }

        [Fact]
        public void Allabolag_Parse_Test()
        {
            var htmlParser = DependenceResolver.Resolver.Container.Resolve<IHtmlParserExt>("AllabologSe");
            var excelWriter = DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Excel");
            var excelFileReader = DependenceResolver.Resolver.Container.Resolve<IExcelReader>();
            var htmlDownload = DependenceResolver.Resolver.Container.Resolve<IStringDownload>("Html");
            var title = new AllabolagSeEntity
            {
                AnnualTurnover = "Annual Turnover",
                CompanyAddress = "Company Address",
                CompanyCity = "Company City",
                CompanyCounty = "Company County",
                CompanyName = "Company Name",
                CompanyPhone = "Company Phone",
                CompanySIC = "Company SIC",
                CompanyZipCode = "Company Zip Code",
                CorporateRegistration = "Corporate Registration",
                NumberOfEmployees = "Number Of Employees"
            };

            var issuccess = excelWriter.SaveInFile(title, @"E:\allabolag\allabolag.xlsx").Result;
            foreach (var regCode in excelFileReader.ReadExcel(@"E:\allabolag\req.xlsx"))
            {
                var htmlFilePath = htmlDownload.Download(string.Format("http://www.allabolag.se/{0}", regCode.First().Replace("-", ""))).Result;
                var htmlParserParameterList = new HtmlParserParameterList
                {
                    Html = File.ReadAllText(htmlFilePath),
                    AdditionalParameterOne = regCode.First()
                };
                var result = htmlParser.ParseHtml(htmlParserParameterList).Result as AllabolagSeEntity;
                issuccess = excelWriter.SaveInFile(result, @"E:\allabolag\allabolag.xlsx").Result;
            }
            Assert.Equal(true, true);
        }

        [Fact]
        public void Tripadviorcom_Lat_lon_Test()
        {
            var htmlParser = DependenceResolver.Resolver.Container.Resolve<IHtmlParserExt>("Tripadvisorcom");
            var excelWriter = DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Excel");
            var excelFileReader = DependenceResolver.Resolver.Container.Resolve<IExcelReader>();
            var htmlDownload = DependenceResolver.Resolver.Container.Resolve<IStringDownload>("Html");
            var title = new TripadvisorcomEntity
            {
                Link = "Link",
                Lon = "Lon",
                Lat = "Lat"
            };
            var issuccess = excelWriter.SaveInFile(title, @"E:\test\Output.xlsx").Result;
            foreach (var regCode in excelFileReader.ReadExcel(@"E:\test\links.xlsx"))
            {
                var htmlFilePath = htmlDownload.Download(regCode.First()).Result;
                var htmlParserParameterList = new HtmlParserParameterList
                {
                    Html = File.ReadAllText(htmlFilePath),
                };
                var result = htmlParser.ParseHtml(htmlParserParameterList).Result as TripadvisorcomEntity;
                result.Link = regCode.First();
                issuccess = excelWriter.SaveInFile(result, @"E:\test\Output.xlsx").Result;
            }
           
            Assert.Equal(true, true);
        }
    }
}
