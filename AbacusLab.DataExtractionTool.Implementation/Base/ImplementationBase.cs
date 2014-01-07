using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Implementation.Annotations;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Base;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.Implementation.Base
{
    public abstract class ImplementationBase : IImplementationBase,INotifyPropertyChanged
    {
        protected readonly IMessageService MessageService;
        protected readonly IFileWriter ExcelWriterService;

        private bool _isEnable;

        protected ImplementationBase()
        {
            MessageService = DependenceResolver.Resolver.Container.Resolve<IMessageService>();
            ExcelWriterService = DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Excel");
        }


        public bool IsEnable
        {
            get { return _isEnable; }
            set
            {
                _isEnable = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
