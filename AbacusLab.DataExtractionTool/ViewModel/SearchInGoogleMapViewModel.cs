using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Base;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.ViewModel
{
    public class SearchInGoogleMapViewModel : DataExtractionBase
    {
        public IGooglePlaceDownload GooglePlaceDownload { get; private set; }

        public SearchInGoogleMapViewModel()
            : base("Search In Google Map")
        {
            GooglePlaceDownload = DependenceResolver.Resolver.Container.Resolve<IGooglePlaceDownload>();
            InitialCommand();

        }

        #region Command

        public RelayCommand<string> ResumeDownloadCommand { get; private set; }
        public RelayCommand<string> SearchInGoogleCommand { get; private set; }
        #endregion

        #region Private Method
        private void InitialCommand()
        {
            SearchInGoogleCommand = new RelayCommand<string>(SearchInGoogleCommandExecute, CommandCanExecute);
            ResumeDownloadCommand = new RelayCommand<string>(ResumeDownloadCommandExecute, CommandCanExecute);
        }

        private async void SearchInGoogleCommandExecute(string searchText)
        {
            IsBusy = true;
            try
            {
                var filePath = DependenceResolver.Resolver.Container.Resolve<IFilePath>("Excel");
                var saveFilePath = filePath.SaveFilePath();
                if (String.IsNullOrEmpty(saveFilePath)) return;
                await GooglePlaceDownload.DownloadPlaceInfo(searchText, saveFilePath);
            }
            finally
            {
                IsBusy = false;
            }

        }

        private void ResumeDownloadCommandExecute(string fileName)
        {
            GooglePlaceDownload.ReadResumeFile(fileName);
        }

        private bool CommandCanExecute(string text)
        {
            return !String.IsNullOrEmpty(text);
        }

        #endregion


    }
}
