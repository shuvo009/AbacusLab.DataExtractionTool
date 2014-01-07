using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AbacusLab.DataExtractionTool.Entitys.Interface;

namespace AbacusLab.DataExtractionTool.Entitys.Html
{
    public class TripadvisorcomEntity : IHtmlEntity
    {
        public TripadvisorcomEntity()
        {
            Lat =
                Link =
                Lon = string.Empty;
        } 
        public string Link { get; set; }
        public string Lat { get; set; }
        public string Lon { get; set; }
      
    }
}
