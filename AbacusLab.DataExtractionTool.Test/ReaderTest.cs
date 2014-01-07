using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Word;
using AbacusLab.DataExtractionTool.Implementation.File;
using AbacusLab.DataExtractionTool.Interface;
using Xunit;
using AbacusLab.DataExtractionTool.Interface.Reader;

namespace AbacusLab.DataExtractionTool.UnitTest
{
    public class ReaderTest
    {
        [Fact]
        public void ExcelFileReadTest()
        {
            IExcelReader fileReader = new ReadFromExcel();
            int i = 0;
            string property = "";
            foreach (var text in fileReader.ReadExcel(@"G:\New folder\success.xlsx"))
            {
                i++;
                property = text.First();
            }
            Assert.Equal("name", property);
            Assert.Equal(1, i);
        }

        [Fact]
        public void TextFileReadText()
        {
            IFileReader fileReader = new ReadFromText();
            int i = 0;
            string property = "";
            foreach (var text in fileReader.ReadFromFile(@"G:\New folder\success.txt"))
            {
                i++;
                property = text;
            }
            Assert.Equal("---------------------------------------", property);
            Assert.Equal(3, i);
        }

        [Fact]
        public void WordFileReadTest()
        {
            IWordReader reader = new WordReader();
            var wordEntity = new WordEntity
            {
                Address = "Address",
                City = "City",
                CompanyName = "CompanyName",
                Fax = "Fax",
                Phone = "Phone",
                State = "State",
                WebSite = "WebSite",
                Zip = "Zip"
            };
            IFileWriter fileWriter = new ExcelWriter();
            var isSuccess = fileWriter.SaveInFile(wordEntity, @"E:\output.xlsx").Result;

            foreach (var tempWordEntity in reader.ReadFromFile(@"E:\Hydroponics_Stores.docx"))
            {
                isSuccess = fileWriter.SaveInFile(tempWordEntity, @"E:\output.xlsx").Result;
            }

            //  var output = reader.ReadFromFile(@"E:\Hydroponics_Stores.docx");
            Assert.Equal(0, 0);
        }

        [Fact]
        public void ExcelFileReader()
        {
            IExcelReader excelReader = new ReadFromExcel();
            var wordEntity = new WordEntity
            {
                Address = "Address",
                City = "City",
                CompanyName = "CompanyName",
                Fax = "Fax",
                Phone = "Phone",
                State = "State",
                WebSite = "WebSite",
                Zip = "Zip"
            };
            IFileWriter fileWriter = new ExcelWriter();
            var isSuccess = fileWriter.SaveInFile(wordEntity, @"E:\output.xlsx").Result;

            var companyInfo = new List<string>();
            foreach (var texts in excelReader.ReadExcel(@"E:\New.xlsx", 1))
            {
                var isEmpty = String.IsNullOrEmpty(texts.First());
                if (!isEmpty)
                {
                    var info = texts.First();
                    if (info.Contains("Web Page"))
                        companyInfo.Add(texts.Last());
                    else
                        companyInfo.Add(info);
                }
                else
                {
                    if (companyInfo.Any())
                    {
                        wordEntity = new WordEntity();

                        wordEntity.CompanyName = companyInfo[0].Trim();
                        wordEntity.Address = companyInfo[1].Trim();
                        var cityAndState = companyInfo[2].Split(',');
                        wordEntity.City = cityAndState[0].Trim();
                        if (cityAndState.Count() >= 2)
                            wordEntity.State = cityAndState[1].Trim();
                        wordEntity.Zip = companyInfo[3].Split(' ')[2].Trim();
                        var nextIndex = 4;
                        var totalInfo = companyInfo.Count() - 1;
                        if (totalInfo >= nextIndex && companyInfo[nextIndex].Contains("Phone"))
                        {
                            wordEntity.Phone = companyInfo[nextIndex].Split(':')[1].Trim();
                            nextIndex++;
                        }
                        else
                        {
                            wordEntity.Phone = string.Empty;
                        }
                        if (totalInfo >= nextIndex && companyInfo[nextIndex].Contains("Fax"))
                        {
                            wordEntity.Fax = companyInfo[nextIndex].Split(':')[1].Trim();
                        }
                        else
                        {
                            wordEntity.Fax = string.Empty;
                        }

                        if (totalInfo >= nextIndex && (companyInfo[nextIndex].StartsWith("http") || companyInfo[nextIndex].StartsWith("www")))
                        {
                            wordEntity.WebSite = companyInfo[nextIndex];
                        }
                        else
                        {
                            wordEntity.WebSite = string.Empty;
                        }
                        isSuccess = fileWriter.SaveInFile(wordEntity, @"E:\output.xlsx").Result;
                        companyInfo = new List<string>();
                    }
                }
            }
        }
    }
}
