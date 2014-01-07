using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Interface;

namespace AbacusLab.DataExtractionTool.Entitys.Html
{
    public class AllabolagSeEntity : IHtmlEntity
    {
        public AllabolagSeEntity()
        {
            CorporateRegistration =
                CompanyName =
                    CompanyAddress =
                        CompanyZipCode =
                            CompanyCity =
                                CompanyPhone =
                                    CompanyCounty =
                                        CompanySIC =
                                            NumberOfEmployees =
                                                AnnualTurnover = string.Empty;
        }
        public string CorporateRegistration { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyZipCode { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyCounty { get; set; }
        public string CompanySIC { get; set; }
        public string NumberOfEmployees { get; set; }
        public string AnnualTurnover { get; set; }
    }
}
