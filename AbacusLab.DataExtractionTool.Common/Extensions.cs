using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbacusLab.DataExtractionTool.Common
{
    public static class Extensions
    {
        public static Dictionary<string, string> ModelToDictionary<T>(this T model)
        {
            var propertyInfo = typeof(T).GetProperties();
            return propertyInfo.Where(x => x.GetValue(model, null) != null)
                                              .ToDictionary(pInfo => pInfo.Name, pInfo => pInfo.GetValue(model, null).ToString());
        }
    }
}
