using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbacusLab.DataExtractionTool.Entitys.Google
{
    public class PlaceInfo
    {
        public string Name { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public string FormattedAddress { get; set; }
        public string WebSite { get; set; }
        public string FormattedPhoneNumber { get; set; }
    }
}
