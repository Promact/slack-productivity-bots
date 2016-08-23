using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.Bot
{
    public class Group
    {



        private MetaData _metaData = null;

        public string creator;
        public string id;
        public bool is_open;
        public string name;
        public List<string> members;

        public Group()
        {
        }

        public Group(MetaData MetaData)
        {
            _metaData = MetaData;
        }

        //public Group(dynamic data)
        //{
        //    id = Utility.TryGetProperty(data, "id");
        //    name = Utility.TryGetProperty(data, "name");
        //    creator = Utility.TryGetProperty(data, "creator");
        //    is_open = Utility.TryGetProperty(data, "is_open", false);
        //    members = new List<String>();
        //    if (Utility.HasProperty(data, "members"))
        //    {
        //        foreach (String strMember in data.members)
        //        {
        //            members.Add(strMember);
        //        }
        //    }
        //}


        public Group(dynamic data)
        {
            id = Utility.TryGetProperty(data, "id");
            name = Utility.TryGetProperty(data, "name");
            creator = Utility.TryGetProperty(data, "creator");
            is_open = Utility.TryGetProperty(data, "is_open", false);
            members = new List<String>();
            //if (Utility.HasProperty(data, "members"))
            //{
            foreach (String strMember in data.members)
            {
                members.Add(strMember);
            }
            //}
        }
    }
}
