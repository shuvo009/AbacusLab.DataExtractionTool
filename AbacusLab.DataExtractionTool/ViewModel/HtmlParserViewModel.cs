using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AbacusLab.DataExtractionTool.Base;
using AbacusLab.DataExtractionTool.Entitys.Html;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using AbacusLab.DataExtractionTool.Interface.Html;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.ViewModel
{
    public class HtmlParserViewModel : DataExtractionBase
    {
        public HtmlParserViewModel()
            : base("Html Parser")
        {
            InitialCommands();
        }

        private void InitialCommands()
        {
            StartParsingCommand = new RelayCommand(StartParsingCommandExecute);
        }

        #region Command

        public RelayCommand StartParsingCommand { get; private set; }
        #endregion

        #region Private Methods

        private async void StartParsingCommandExecute()
        {
            IsBusy = true;
            try
            {
                ProgressValue = 0;
                var fileBrowser = DependenceResolver.Resolver.Container.Resolve<IFilePath>("Excel");
                var saveFilePath = fileBrowser.SaveFilePath();
                if (String.IsNullOrEmpty(saveFilePath))
                    return;
                fileBrowser = DependenceResolver.Resolver.Container.Resolve<IFilePath>("Text");
                var htmlFileLocation = fileBrowser.ReadFilePath();
                if (String.IsNullOrEmpty(htmlFileLocation))
                    return;
                var htmlParser = DependenceResolver.Resolver.Container.Resolve<IHtmlParser>();
                var mainLinks =
                    await
                        htmlParser.ExtractHrefFromTable(File.ReadAllText(htmlFileLocation), "MainContent", 4, 1,
                            @"http://idaman2.kpkt.gov.my:8888/idv5/98_eHome/{0}");
                var htmlDownload = DependenceResolver.Resolver.Container.Resolve<IStringDownload>("Html");
                IList<IFileWriter> writers =new List<IFileWriter>
                {
                     DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Csv"),
                     DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Excel"),
                     DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Text"),
                };

                var header = new CommonEntity
                {
                    NoKpOrPassport = "NoKpOrPassport",
                    Alamat = "Alamat",
                    Negeri = "Negeri",
                    Daerah = "Daerah",
                    Bandar = "Bandar",
                    Poskod = "Poskod",
                    Kerakyatan = "Kerakyatan",
                    Jawatan = "Jawatan",
                    TarikhLantikanJawatan = "TarikhLantikanJawatan",
                    SenaraiHitam = "SenaraiHitam"
                };

                foreach (var fileWriter in writers)
                {
                    await fileWriter.SaveInFile(header, saveFilePath);
                }
               

                MaxProgressValue = mainLinks.Count;
                foreach (var mainLink in mainLinks)
                {
                    var htmlFilePath = await htmlDownload.Download(mainLink);
                    var informationLinks =
                        await
                            htmlParser.ExtractHrefFromTable(File.ReadAllText(htmlFilePath), "MainContent",
                                tablePosition: 1, urlFormat: UrlMaker(mainLink));
                    foreach (var informationLink in informationLinks)
                    {
                        htmlFilePath = await htmlDownload.Download(informationLink);
                        var information =
                            await
                                htmlParser.ExtractHrefFromTable(File.ReadAllText(htmlFilePath), "MainContent",
                                    columnPosition: 1, tablePosition: 0, isAttribute: false);
                        var infoForSave = new CommonEntity
                        {
                            NoKpOrPassport = information[0],
                            Alamat = information[1],
                            Negeri = information[2],
                            Daerah = information[3],
                            Bandar = information[4],
                            Poskod = information[5],
                            Kerakyatan = information[6],
                            Jawatan = information[7],
                            TarikhLantikanJawatan = information[8],
                            SenaraiHitam = information[9],
                        };
                        foreach (var fileWriter in writers)
                        {
                            await fileWriter.SaveInFile(infoForSave, saveFilePath);
                        }
                    }
                    ProgressValue++;
                }
                MessagesService.ShowMessages("Work Complete !!!");
            }
            catch (Exception ex)
            {
                MessagesService.ShowMessages(ex.Message);
            }
            finally
            {
                IsBusy = false;
            }
        }


        private string UrlMaker(string mainUrl)
        {
            const string indexCutter = "pmju_kod=";
            var id = mainUrl.Substring(mainUrl.IndexOf(indexCutter, System.StringComparison.Ordinal) + indexCutter.Length);
            return
                String.Format(
                    "http://idaman2.kpkt.gov.my:8888/idv5/98_eHome/template/pengarahDetails.cfm?pmju_Kod={0}&proj_Kod_Fasa=1&rekid={1}",
                    id, "{0}");
        }

        #endregion
    }
}
