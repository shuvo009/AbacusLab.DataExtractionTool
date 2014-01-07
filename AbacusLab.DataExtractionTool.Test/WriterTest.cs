using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Implementation.File;
using AbacusLab.DataExtractionTool.Interface;
using Microsoft.Practices.Unity;
using Xunit;
namespace AbacusLab.DataExtractionTool.UnitTest
{

    public class WriterTest
    {
        [Fact]
        public  void SaveIntoExcelFileTest()
        {
            IFileWriter fileWriter = new ExcelWriter();
            var testInfo = new TestModel { Name = "name", Roll = "roll" };
            var isSuccess = fileWriter.SaveInFile(testInfo, @"G:\New folder\success.xlsx").Result;
            Assert.True(isSuccess);
        }

        [Fact]
        public void SaveInTextFileTest()
        {
            IFileWriter fileWriter = new TextWriter();
            var testInfo = new TestModel { Name = "name", Roll = "roll" };
            var isSuccess = fileWriter.SaveInFile(testInfo, @"G:\New folder\success.txt").Result;
            Assert.True(isSuccess);
        }
    }

    public class TestModel
    {
        public string Name { get; set; }
        public string Roll { get; set; }
    }
}
