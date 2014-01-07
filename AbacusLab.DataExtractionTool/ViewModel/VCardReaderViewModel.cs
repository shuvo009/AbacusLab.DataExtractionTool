using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using AbacusLab.DataExtractionTool.Base;
using AbacusLab.DataExtractionTool.Entitys;
using AbacusLab.DataExtractionTool.Implementation.Dependence;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Download;
using Microsoft.Practices.Unity;

namespace AbacusLab.DataExtractionTool.ViewModel
{
    public class VCardReaderViewModel : DataExtractionBase
    {
        public VCardReaderViewModel()
            : base("VCard Reader")
        {
            InitialActions();
        }

        #region Private Methods

        private void InitialActions()
        {
            StartAction = StartRead;
        }

        private async void StartRead()
        {
            IsBusy = true;
            try
            {
                var filePath = DependenceResolver.Resolver.Container.Resolve<IFilePath>("Excel");
                var readFilePath = filePath.ReadFilePath();
                if (string.IsNullOrEmpty(readFilePath))
                    return;
                var saveFilePath = filePath.SaveFilePath();
                if (string.IsNullOrEmpty(saveFilePath))
                    return;
                var excelReader = DependenceResolver.Resolver.Container.Resolve<IExcelReader>();
                MaxProgressValue = excelReader.TotalRows(readFilePath);
                var vCard = DependenceResolver.Resolver.Container.Resolve<IStringDownload>("VCard");
                var textReader = DependenceResolver.Resolver.Container.Resolve<IFileReader>();
                var writers = new List<IFileWriter>
                {
                    DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Text"),
                    DependenceResolver.Resolver.Container.Resolve<IFileWriter>("Excel")
                };
                foreach (var row in excelReader.ReadExcel(readFilePath, columnNumber: 1))
                {
                    Uri downloadLink;
                    if (!Uri.TryCreate(row.First(), UriKind.Absolute, out downloadLink)) continue;
                    var vCardPath = await vCard.Download(downloadLink.ToString());
                    var vCardEntity = new VCardEntity();
                    foreach (var line in textReader.ReadFromFile(vCardPath))
                    {
                        ExtractData(line, vCardEntity);
                    }
                    foreach (var fileWriter in writers)
                    {
                        await fileWriter.SaveInFile(vCardEntity, saveFilePath);
                    }
                    ProgressValue++;
                }
                MessagesService.ShowMessages("Work Complete !!!");
            }
            finally
            {
                IsBusy = false;

            }
        }

        private void ExtractData(string line, VCardEntity vCardEntity)
        {

            if (line.StartsWith("N:")) // For V2.1 : N:
            {
                GetNames(line, vCardEntity);
            }
            else if (line.StartsWith("TITLE:"))
            {
                vCardEntity.Title = GetSecondElement(line);
            }
            else if (line.StartsWith("EMAIL;PREF;INTERNET:")) // For V2.1 : EMAIL;PREF;INTERNET:
            {
                vCardEntity.Email = GetSecondElement(line);
            }
            else if (line.StartsWith("TEL;WORK;VOICE:")) // For V2.1 : TEL;WORK;VOICE:
            {
                vCardEntity.Tel = GetSecondElement(line);
            }
            else if (line.StartsWith("ADR;WORK;PREF:")) // For V2.1 : ADR;WORK;ENCODING=QUOTED-PRINTABLE:
            {
                GetCityAndCountry(line, vCardEntity);
            }
            else if (line.StartsWith("URL;WORK:"))
            {
                vCardEntity.ProfileLink = GetProfileLink(line);
            }
        }

        private void GetCityAndCountry(string line, VCardEntity currentPerson)
        {
            try
            {
                //using (var sr = new StreamReader("vcard.vcf"))
                //{
                //    var vCardaddress = sr.ReadToEnd();
                //    var templine = vCardaddress.Substring(vCardaddress.IndexOf("ADR;WORK;ENCODING=QUOTED-PRINTABLE:", System.StringComparison.Ordinal));
                //   // var tenp2Line = templine.Substring(templine.IndexOf("LABEL;WORK;ENCODING=QUOTED-PRINTABLE:", System.StringComparison.Ordinal));
                //    var tenp2Line = templine.Substring(templine.LastIndexOf("=0D=0A", System.StringComparison.Ordinal));

                //    line = templine.Substring(0, templine.Length - tenp2Line.Length);
                //}

                var address = GetSecondElement(line);
                currentPerson.Address = address;
                var cityAndCountry = address.Split(';');
                //var cityAndCountry = address.Split(',');
                var columnNumber = cityAndCountry.Count();

                // For Baker & McKenzie
                //currentPerson.Country = cityAndCountry[columnNumber - 2].Substring(cityAndCountry[columnNumber - 2].IndexOf("0A", System.StringComparison.Ordinal) + 2);
                currentPerson.Country = cityAndCountry[columnNumber - 2];

                //var citys = cityAndCountry[columnNumber - 3].Split(',');
                //if(citys.Count()<=2)
                //{
                //    currentPerson.City = citys[0];
                //}
                //else
                //{
                //    currentPerson.City = cityAndCountry[columnNumber - 3];
                //}


                //  currentPerson.Country = string.IsNullOrEmpty(cityAndCountry.Last()) ? cityAndCountry[2] : cityAndCountry.Last();
                if (currentPerson.Country.Equals("United States of America") || currentPerson.Country.Equals("United States") || currentPerson.Country.Equals("US"))
                {
                    //currentPerson.City = cityAndCountry[columnNumber - 4];
                    //currentPerson.City = String.IsNullOrEmpty(cityAndCountry[columnNumber - 4]) ? cityAndCountry[columnNumber - 3] : cityAndCountry[columnNumber - 4];
                    currentPerson.City = String.IsNullOrEmpty(cityAndCountry[columnNumber - 5]) ? cityAndCountry[columnNumber - 4] : cityAndCountry[columnNumber - 5];
                }
                else
                {
                    currentPerson.City = cityAndCountry[columnNumber - 4];
                }
                // For DLA Piper and Kirkland_&_Ellis
                //currentPerson.City = cityAndCountry[columnNumber - 4];
                //currentPerson.Country = cityAndCountry[columnNumber - 2];

                // Kirkland_&_Ellis
                //currentPerson.City = cityAndCountry[columnNumber - 3];
                //currentPerson.Country = cityAndCountry[columnNumber - 1];

                // Skadden
                //currentPerson.City = String.IsNullOrEmpty(cityAndCountry[columnNumber - 3]) ? cityAndCountry[columnNumber - 4] : cityAndCountry[columnNumber - 3];
                //currentPerson.Country = cityAndCountry[columnNumber - 1];

            }
            finally
            {
            }
        }

        private string GetSecondElement(string line)
        {
            try
            {
                return String.IsNullOrEmpty(line) ? string.Empty : line.Split(':')[1];
            }
            catch (Exception)
            {

                return "";
            }
        }

        private void GetNames(string nameLine, VCardEntity vCardEntity)
        {
            try
            {
                var names = GetSecondElement(nameLine).Split(';');
                vCardEntity.FirstName = names[1];
                vCardEntity.LastName = names[0];
                if (names.Count() > 2)
                    vCardEntity.MiddleName = names[2];
            }
            finally
            {
            }
        }

        private string GetProfileLink(string line)
        {
            var links = line.Split(':');
            if (links.Count() >= 3)
            {
                return string.Format("{0}:{1}", links[1], links[2]);
            }
            return string.Empty;
        }
        #endregion
    }
}
