using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Implementation.Download;
using AbacusLab.DataExtractionTool.Implementation.Download.Amazon;
using AbacusLab.DataExtractionTool.Implementation.Download.Google;
using AbacusLab.DataExtractionTool.Implementation.File;
using AbacusLab.DataExtractionTool.Implementation.FilePath;
using AbacusLab.DataExtractionTool.Implementation.Html;
using AbacusLab.DataExtractionTool.Implementation.Messages;
using AbacusLab.DataExtractionTool.Implementation.RegexParser;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using AbacusLab.DataExtractionTool.Interface.Html;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.Implementation.Dependence
{
    public class DependenceResolver
    {
        public IUnityContainer Container;
        private DependenceResolver()
        {
            Container = new UnityContainer();
            Register();
        }

        private static DependenceResolver Dependence { get; set; }

        public static DependenceResolver Resolver
        {
            get { return Dependence ?? (Dependence = new DependenceResolver()); }
        }

        #region Private
        private void Register()
        {
            Container.RegisterType<IFilePath, TextFilePath>("Text");
            Container.RegisterType<IFilePath, ExcelFilePath>("Excel");

            Container.RegisterType<IFileWriter, TextWriter>("Text");
            Container.RegisterType<IFileWriter, ExcelWriter>("Excel");
            Container.RegisterType<IFileWriter, CsvWriter>("Csv");

            Container.RegisterType<IFileReader, ReadFromText>();
            Container.RegisterType<IExcelReader, ReadFromExcel>();

            Container.RegisterType<IStringDownload, DownloadHtml>("Html");
            Container.RegisterType<IStringDownload, DownloadVCard>("VCard");

            Container.RegisterType<IRegexParser, ParseEmail>("Email");
            Container.RegisterType<IRegexParser, ParserWebSite>("WebSite");

            Container.RegisterType<IAmazonProductInfoDownloaded, AmazonProductInfoDownloaded>();
            Container.RegisterType<IMessageService, MessageService>();
            Container.RegisterType<IDynamicDownload, DynamicDownload>();
            Container.RegisterType<IGooglePlaceDownload, GooglePlaceDownload>();
            Container.RegisterType<IHtmlParser, HtmlParser>();


            Container.RegisterType<IHtmlParserExt, AllabologSe>("AllabologSe");
            Container.RegisterType<IHtmlParserExt, Tripadvisorcom>("Tripadvisorcom");
        }
        #endregion
    }
}
