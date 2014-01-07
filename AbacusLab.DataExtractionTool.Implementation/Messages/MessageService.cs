using System.Windows;
using AbacusLab.DataExtractionTool.Interface;

namespace AbacusLab.DataExtractionTool.Implementation.Messages
{
    public class MessageService : IMessageService
    {
        public void ShowMessages(string messages)
        {
            MessageBox.Show(messages);
        }

        public bool ShowMessagesYesNo(string messages)
        {
            return MessageBox.Show(messages, "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
    }
}