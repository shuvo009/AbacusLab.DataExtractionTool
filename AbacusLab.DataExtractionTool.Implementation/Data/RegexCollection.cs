using System.Collections.Generic;

namespace AbacusLab.DataExtractionTool.Implementation.Data
{
    public static class RegexCollection
    {
        public static List<string> EmailRegex = new List<string>
        {
            @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
            @"^\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}$",
            @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$",
            @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            @"[\w-]+@([\w-]+\.)+[\w-]+",
            @"^\w+[\w-\.]*\@\w+((-\w+)|(\w*))\.[a-z]{2,3}$",
            @"^[\w\.=-]+@[\w\.-]+\.[\w]{2,3}$",
            @"(\w+?@\w+?\x2E.+)",
            @"^(([-\w \.]+)|(&quot;&quot;[-\w \.]+&quot;&quot;) )?&lt;([\w\-\.]+)@((\[([0-9]{1,3}\.){3}[0-9]{1,3}\])|(([\w\-]+\.)+)([a-zA-Z]{2,4}))&gt;$"
        };

        public static List<string> WebSiteRegex = new List<string>
        {
            @"(www.+|http(s)?://+)([\w-]+\.)+[\w-]+(/[\w ./?%&=]*)?",
            @"(www.+|http.+)([\s]|$)",
            @"((mailto\:|(news|(ht|f)tp(s?))\://){1}\S+)"
        };
    }
}