using System.Collections.Generic;
using System.Web;

namespace Vctc_CSharp_Sdk.Utils
{
    public class HtmlUtils
    {
        public static string BuildQueryString(SortedList<string,string> pairs)
        {
            string query = "";
            foreach (var k in pairs.Keys)
            {
                if (query!="")
                {
                    query += "&";
                }
                query+=k+"="+ pairs[k];
            }

            return query;
        }
    }
}