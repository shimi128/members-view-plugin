using System.Collections.Generic;
using Umbraco.Core.Models;

namespace MembersPlugin.Models
{
    public class CustomMember
    {
        public IEnumerable<Member> Members { get; set; }
        public int TotlalRecords { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IList<KeyValuePair<string, string>> PropertiesList { get; set; }

        public CustomMember()
        {
            PropertiesList=new List<KeyValuePair<string, string>>();
        }
    }
}