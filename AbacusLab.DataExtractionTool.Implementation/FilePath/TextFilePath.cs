using AbacusLab.DataExtractionTool.Interface;
using Microsoft.Win32;

namespace AbacusLab.DataExtractionTool.Implementation.FilePath
{
    public class TextFilePath : IFilePath
    {
        public string ReadFilePath()
        {
            var dlg = new OpenFileDialog {DefaultExt = ".txt", Filter = "Text (.txt)|*.txt"};
            return dlg.ShowDialog() == true ? dlg.FileName : string.Empty;
        }

        public string SaveFilePath()
        {
            var dlS = new SaveFileDialog {DefaultExt = ".txt", Filter = "Excel (.txt)|*.txt"};
            return dlS.ShowDialog() == true ? dlS.FileName : string.Empty;
        }
    }
}