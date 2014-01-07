using AbacusLab.DataExtractionTool.Interface;
using Microsoft.Win32;

namespace AbacusLab.DataExtractionTool.Implementation.FilePath
{
    public class ExcelFilePath : IFilePath
    {
        public string ReadFilePath()
        {
            var dlg = new OpenFileDialog {DefaultExt = ".xlsx", Filter = "Excel (.xlsx)|*.xlsx"};
            return dlg.ShowDialog() == true ? dlg.FileName : string.Empty;
        }

        public string SaveFilePath()
        {
            var dlS = new SaveFileDialog {DefaultExt = ".xlsx", Filter = "Excel (.xlsx)|*.xlsx"};
            return dlS.ShowDialog() == true ? dlS.FileName : string.Empty;
        }
    }
}