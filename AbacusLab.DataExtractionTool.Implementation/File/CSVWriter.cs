using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using AbacusLab.DataExtractionTool.Common;
using AbacusLab.DataExtractionTool.Interface;

namespace AbacusLab.DataExtractionTool.Implementation.File
{
    public class CsvWriter : IFileWriter
    {
        public Task<bool> SaveInFile<T>(T data, string filePath)
        {
            return Task.Run(() =>
            {
                filePath = System.IO.Path.ChangeExtension(filePath, "csv");
                if (!System.IO.File.Exists(filePath))
                    CreateFile(filePath);
                var textWriter = new System.IO.StreamWriter(filePath, true);
                textWriter.WriteLine(String.Join(", ", data.ModelToDictionary().Select(x => x.Value.Replace(",", ""))));
                textWriter.Flush();
                textWriter.Close();
                return true;
            });
        }

        #region Private Method

        private void CreateFile(string filePath)
        {
            using (System.IO.File.CreateText(filePath)) { }
        }

        #endregion
    }
}
