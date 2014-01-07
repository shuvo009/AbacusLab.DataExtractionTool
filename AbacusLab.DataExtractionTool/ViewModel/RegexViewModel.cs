using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Base;
using AbacusLab.DataExtractionTool.Entitys.Regex;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.ViewModel
{
    public class RegexViewModel : DataExtractionBase
    {
        public RegexViewModel()
            : base("Reg ex Parser")
        {
            InitialAction();
        }

        #region Private

        private void InitialAction()
        {
            StartAction = StartCommandExecution;
        }

        private async void StartCommandExecution()
        {
            IsBusy = true;
            ProgressValue = 0;
            try
            {
                var filePathService = DependenceResolver.Resolver.Container.Resolve<IFilePath>("Excel");
                var openFilePath = filePathService.ReadFilePath();
                if (String.IsNullOrEmpty(openFilePath))
                    return;
                var fileSavePath = filePathService.SaveFilePath();
                if (string.IsNullOrEmpty(fileSavePath))
                    return;
                var readFileService = DependenceResolver.Resolver.Container.Resolve<IExcelReader>();
                MaxProgressValue = readFileService.TotalRows(openFilePath);
                var fileWriteService = DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Excel");
                var emailService = DependenceResolver.Resolver.Container.Resolve<IRegexParser>("Email");
                var webSiteService = DependenceResolver.Resolver.Container.Resolve<IRegexParser>("WebSite");
                foreach (var row in readFileService.ReadExcel(openFilePath))
                {
                    var regexCommonEntity = new RegexCommon
                    {
                        Email = emailService.Parse(row.First()),
                        WebSite = webSiteService.Parse(row.First())
                    };
                    await fileWriteService.SaveInFile(regexCommonEntity, fileSavePath);
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

        #endregion
    }
}
