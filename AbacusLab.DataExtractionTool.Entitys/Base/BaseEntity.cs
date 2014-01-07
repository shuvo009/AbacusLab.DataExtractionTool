using System.ComponentModel;
using System.Runtime.CompilerServices;
using AbacusLab.DataExtractionTool.Entitys.Annotations;

namespace AbacusLab.DataExtractionTool.Entitys.Base
{
    public class BaseEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}