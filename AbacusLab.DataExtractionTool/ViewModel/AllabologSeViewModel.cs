using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Base;
using AbacusLab.DataExtractionTool.Entitys.Html;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using AbacusLab.DataExtractionTool.Interface.Html;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.ViewModel
{
    public class AllabologSeViewModel : DataExtractionBase
    {
        public AllabologSeViewModel()
            : base("Allabolog.Se")
        {
            InitialAction();
        }

        private void InitialAction()
        {
            StartAction = StatCommandExecute;
        }

        private async void StatCommandExecute()
        {
            IsBusy = true;
            ProgressValue = 0;
            try
            {
                var htmlParser = DependenceResolver.Resolver.Container.Resolve<IHtmlParserExt>("AllabologSe");
                var excelWriter = DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Excel");
                var excelFileReader = DependenceResolver.Resolver.Container.Resolve<IExcelReader>();
                var htmlDownload = DependenceResolver.Resolver.Container.Resolve<IStringDownload>("Html");
                var filePathService = DependenceResolver.Resolver.Container.Resolve<IFilePath>("Excel");

                var openFilePath = filePathService.ReadFilePath();
                if (String.IsNullOrEmpty(openFilePath))
                    return;
                var fileSavePath = filePathService.SaveFilePath();
                if (string.IsNullOrEmpty(fileSavePath))
                    return;

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
                MaxProgressValue = excelFileReader.TotalRows(openFilePath);
                await excelWriter.SaveInFile(title, fileSavePath);
                foreach (var regCode in excelFileReader.ReadExcel(openFilePath))
                {
                    var htmlFilePath = await htmlDownload.Download(string.Format("http://www.allabolag.se/{0}", regCode.First().Replace("-", "")));
                    var htmlParserParameterList = new HtmlParserParameterList
                    {
                        Html = File.ReadAllText(htmlFilePath),
                        AdditionalParameterOne = regCode.First()
                    };
                    var result = await htmlParser.ParseHtml(htmlParserParameterList) as AllabolagSeEntity;
                    await excelWriter.SaveInFile(result, @"E:\allabolag\allabolag.xlsx");
                    ProgressValue++;
                }
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
    }
}
