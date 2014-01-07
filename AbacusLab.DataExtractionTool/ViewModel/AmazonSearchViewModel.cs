using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Base;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Implementation.Download.Amazon;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.ViewModel
{
    public class AmazonSearchViewModel : DataExtractionBase
    {

        public IAmazonProductInfoDownloaded AmazonProductInfoDownloaded { get; set; }
        private readonly IFilePath _filePath;
        private bool _isIsbn = true;

        public AmazonSearchViewModel()
            : base("Amazon Search")
        {
            _filePath = DependenceResolver.Resolver.Container.Resolve<IFilePath>("Excel");
            AmazonProductInfoDownloaded = DependenceResolver.Resolver.Container.Resolve<IAmazonProductInfoDownloaded>();
            InitialAction();
            InitialCommands();
        }

        private void InitialCommands()
        {
            SearchByTextCommand = new RelayCommand<string>(SearchByTextCommandExecute, SearchByTextCommandCanExecute);
        }

        private void InitialAction()
        {
            StartAction = SearchByIsbn;
        }

        #region Property

        public bool IsIsbn
        {
            get { return _isIsbn; }
            set
            {
                _isIsbn = value;
                RaisePropertyChanged(() => IsIsbn);
            }
        }

        public AmazonSearchIndex AmazonSearchIndex { get; set; }

        #endregion

        #region Command

        public RelayCommand<string> SearchByTextCommand { get; private set; }
        #endregion

        #region Private Methods

        private async void SearchByIsbn()
        {
            IsBusy = true;
            try
            {
                var openFilePath = _filePath.ReadFilePath();
                if (String.IsNullOrEmpty(openFilePath)) return;
                var saveFilePath = _filePath.SaveFilePath();
                if (String.IsNullOrEmpty(saveFilePath)) return;
                var excelFileReader = DependenceResolver.Resolver.Container.Resolve<IExcelReader>();
                MaxProgressValue = excelFileReader.TotalRows(saveFilePath);
                ProgressValue = 0;
                foreach (var row in excelFileReader.ReadExcel(openFilePath))
                {
                    var isSuccess = await AmazonProductInfoDownloaded.DownloadProductByLookup(row.First(), IsIsbn, saveFilePath);
                    if (!isSuccess) return;
                    ProgressValue++;
                }
                MessagesService.ShowMessages("Work Complete !!!!");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void SearchByTextCommandExecute(string searchText)
        {
            IsBusy = true;
            try
            {
                ProgressValue = 0;
                MaxProgressValue = AmazonProductInfoDownloaded.ProgressMaxValue;
                ProgressValue = AmazonProductInfoDownloaded.ProgressComplete;
                var saveFilePath = _filePath.SaveFilePath();
                if (String.IsNullOrEmpty(saveFilePath)) return;
                var isSuccess = await AmazonProductInfoDownloaded.DownloadProductBySearch(searchText, AmazonSearchIndex.ToString(), saveFilePath);
                if (!isSuccess) return;
                MessagesService.ShowMessages("Work Complete!!!!");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool SearchByTextCommandCanExecute(string searchText)
        {
            return !String.IsNullOrEmpty(searchText);
        }

        #endregion
    }
}
