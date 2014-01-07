using AbacusLab.DataExtractionTool.Entitys.Word;
using AbacusLab.DataExtractionTool.Interface;
using AbacusLab.DataExtractionTool.Interface.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Word;
using Telerik.Windows.DragDrop.Behaviors;

namespace AbacusLab.DataExtractionTool.Implementation.File
{
    public class WordReader : IWordReader
    {
        public IEnumerable<WordEntity> ReadFromFile(string filePath)
        {
            var word = new Application();
            object miss = System.Reflection.Missing.Value;
            object path = filePath;
            object readOnly = true;

            var docs = word.Documents.Open(ref path, ref miss, ref readOnly,
                                       ref miss, ref miss, ref miss, ref miss,
                                       ref miss, ref miss, ref miss, ref miss,
                                       ref miss, ref miss, ref miss, ref miss,
                                       ref miss);

            docs.Activate();
            docs.SelectAllEditableRanges();
            Hyperlinks hLinks = docs.Hyperlinks;
            var hLinkNumber = 1;

            //foreach (Range storyRange in docs.StoryRanges)
            //{
            Range storyRange = docs.Range();
            var text = storyRange.Text;
            var info = text.Split(new string[] { " mi" }, StringSplitOptions.None);
            foreach (var singleInfoLoop in info)
            {
                var singleInfo = singleInfoLoop.Replace("\a", "").Replace("\r", "");
                singleInfo = singleInfo.Trim();
                if (String.IsNullOrEmpty(singleInfo) || String.IsNullOrWhiteSpace(singleInfo) || singleInfo.Length < 10) continue;

                var wordEntity = new WordEntity();
                var companyInfo = singleInfo.Split(new string[] { @"" }, StringSplitOptions.None).ToList();
                foreach (var cmp in companyInfo.ToList())
                {
                    if (String.IsNullOrWhiteSpace(cmp) || String.IsNullOrEmpty(cmp.Trim()) || String.Equals("/", cmp.Trim()) || cmp.Length < 3)
                        companyInfo.Remove(cmp);
                }


                wordEntity.CompanyName = companyInfo[0].Trim();
                wordEntity.Address = companyInfo[1].Trim();
                var cityAndState = companyInfo[2].Split(',');
                wordEntity.City = cityAndState[0].Trim();
                if (cityAndState.Count() >= 2)
                    wordEntity.State = cityAndState[1].Trim();
                wordEntity.Zip = companyInfo[3].Split(' ')[2].Trim();
                var nextIndex = 4;
                if (singleInfo.Contains("Phone"))
                {
                    wordEntity.Phone = companyInfo[nextIndex].Split(':')[1].Trim();
                    nextIndex++;
                }
                else
                {
                    wordEntity.Phone = string.Empty;
                }
                if (singleInfo.Contains("Fax"))
                {
                    wordEntity.Fax = companyInfo[nextIndex].Split(':')[1].Trim();
                }
                else
                {
                    wordEntity.Fax = string.Empty;
                }

                if (singleInfo.Contains("Web Page"))
                {
                    // var hLinkNumberRef = (object)hLinkNumber;
                    try
                    {
                        wordEntity.WebSite = hLinks[hLinkNumber].Address;

                    }
                    catch (Exception)
                    {

                    }
                    hLinkNumber++;
                }
                else
                {
                    wordEntity.WebSite = string.Empty;
                }

                yield return wordEntity;
            }
            // }
        }


        private bool EmptyCheck(string text)
        {
            return !(String.IsNullOrEmpty(text) || String.IsNullOrWhiteSpace(text));
        }

        public double TotalLine(string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
