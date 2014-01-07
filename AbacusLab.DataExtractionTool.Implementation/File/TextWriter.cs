using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Common;
using AbacusLab.DataExtractionTool.Interface;
using FileIO = System.IO;
namespace AbacusLab.DataExtractionTool.Implementation.File
{
    public class TextWriter : IFileWriter
    {
        public Task<bool> SaveInFile<T>(T data, string filePath)
        {
            return Task.Run(() =>
            {
                try
                {
                    filePath = FileIO.Path.ChangeExtension(filePath, "txt");
                    if (!FileIO.File.Exists(filePath))
                        CreateFile(filePath);
                    var textWriter = new FileIO.StreamWriter(filePath, true);
                    foreach (var property in data.ModelToDictionary())
                    {
                        textWriter.WriteLine("{0} : {1}", property.Key, property.Value);
                    }
                    textWriter.WriteLine("---------------------------------------");
                    textWriter.Flush();
                    textWriter.Close();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        #region Private Method

        private void CreateFile(string filePath)
        {
            using (FileIO.File.CreateText(filePath)) { }
        }

        #endregion
    }
}
