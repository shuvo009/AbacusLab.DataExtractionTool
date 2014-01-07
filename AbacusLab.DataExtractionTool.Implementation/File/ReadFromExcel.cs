using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Interface;
using ClosedXML.Excel;

namespace AbacusLab.DataExtractionTool.Implementation.File
{
    public class ReadFromExcel : IExcelReader
    {

        public IEnumerable<List<string>> ReadExcel(string filePath, int columnNumber = 1)
        {
            var workbook = new XLWorkbook(filePath);
            var ws = workbook.Worksheet(1);
            foreach (var xlRow in ws.Rows())
            {
                yield return Enumerable.Range(1, columnNumber).Select(i => xlRow.Cell(i).HasHyperlink ? (xlRow.Cell(i).GetHyperlink().ExternalAddress != null ? xlRow.Cell(i).GetHyperlink().ExternalAddress.ToString() : " _ ") : xlRow.Cell(i).Value.ToString()).ToList();
            }
            workbook.Dispose();
        }

        public double TotalRows(string filePath)
        {
            double rows = 0;
            var workbook = new XLWorkbook(filePath);
            var ws = workbook.Worksheet(1);
            if (ws.Rows().LastOrDefault() != null)
                rows = ws.Rows().Last().RowNumber();
            workbook.Dispose();
            return rows;
        }
    }
}
