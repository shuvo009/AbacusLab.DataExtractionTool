namespace AbacusLab.DataExtractionTool.Interface
{
    public interface IMessageService
    {
        void ShowMessages(string messages);
        bool ShowMessagesYesNo(string messages);
    }
}