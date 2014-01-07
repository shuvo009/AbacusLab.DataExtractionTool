using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.Base
{
    public abstract class DataExtractionBase : ViewModelBase
    {
        private bool _isBusy;
        private string _title;
        private string _busyContent = "Processing...";
        private double _progressValue;
        private double _maxProgressValue = 100;
        protected IMessageService MessagesService;
        protected DataExtractionBase(string title)
        {
            Title = title;
            InitialCommands();
            MessagesService = DependenceResolver.Resolver.Container.Resolve<IMessageService>();
        }

        protected Action StartAction;

        #region Command

        public RelayCommand StartCommand { get; private set; }
        #endregion

        #region Private Methods

        private void InitialCommands()
        {
            StartCommand = new RelayCommand(StartCommandExecution);
        }

        private void StartCommandExecution()
        {
            StartAction();
        }

        #endregion

        #region Property
        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public string BusyContent
        {
            get { return _busyContent; }
            set
            {
                _busyContent = value;
                RaisePropertyChanged(() => BusyContent);
            }
        }

        public double ProgressValue
        {
            get { return _progressValue; }
            set
            {
                _progressValue = value;
                RaisePropertyChanged(() => ProgressValue);
            }
        }

        public double MaxProgressValue
        {
            get { return _maxProgressValue; }
            set
            {
                _maxProgressValue = value;
                RaisePropertyChanged(() => MaxProgressValue);
            }
        }
        #endregion
    }
}
