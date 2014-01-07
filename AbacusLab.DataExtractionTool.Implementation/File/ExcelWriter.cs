using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Common;
using AbacusLab.DataExtractionTool.Interface;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using FileIO = System.IO;

namespace AbacusLab.DataExtractionTool.Implementation.File
{
    public class ExcelWriter : IFileWriter
    {
        public async Task<bool> SaveInFile<T>(T data, string filePath)
        {
            return await Task.Run(() =>
            {
                try
                {
                    filePath = FileIO.Path.ChangeExtension(filePath, "xlsx");
                    if (!FileIO.File.Exists(filePath))
                        CreateExcelFile(filePath);
                    var saveIntoExcel = new XLWorkbook(filePath);
                    var excelWorkShit = saveIntoExcel.Worksheet(1);
                    var rowNumber = 1;
                    if (excelWorkShit.Rows().LastOrDefault() != null)
                        rowNumber = excelWorkShit.Rows().Last().RowNumber() + 1;
                    var cellNumber = 1;
                    foreach (var property in data.ModelToDictionary())
                    {
                        excelWorkShit.Cell(rowNumber, cellNumber).DataType = XLCellValues.Text;
                        excelWorkShit.Cell(rowNumber, cellNumber).Value = property.Value;
                        cellNumber++;
                    }
                    saveIntoExcel.Save();
                    saveIntoExcel.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        #region Private Methods

        private void CreateExcelFile(string filePath)
        {
            var createExcelFile = new XLWorkbook();
            createExcelFile.Worksheets.Add("Data");
            createExcelFile.SaveAs(filePath);
        }

        #endregion
    }
}