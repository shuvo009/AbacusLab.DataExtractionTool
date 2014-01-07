using AbacusLab.DataExtractionTool.Interface.Base;

namespace AbacusLab.DataExtractionTool.Implementation.Base
{
    public abstract class DownloadBase : ImplementationBase, IDownloadBase
    {
        private bool _isIndeterminate;
        private double _progressComplete;
        private double _progressMaxValue;

        public double ProgressMaxValue
        {
            get { return _progressMaxValue; }
            set
            {
                _progressMaxValue = value;
                OnPropertyChanged();
            }
        }

        public double ProgressComplete
        {
            get { return _progressComplete; }
            set
            {
                _progressComplete = value;
                OnPropertyChanged();
            }
        }

        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set
            {
                _isIndeterminate = value;
                OnPropertyChanged();
            }
        }
    }
}