using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.Bot
{
    public class Channel
    {
        private MetaData _metaData = null;
        
        public string creator;
        public bool has_pins;
        public string id;
        public bool is_archived;
        public bool is_channel;
        public bool is_general;
        public bool is_member;
        public string name;
        public List<string> members;
        public int unread_count;
        public int unread_count_display;


        public Channel()
        {
        }

        public Channel(MetaData MetaData)
        {
            _metaData = MetaData;
        }

        //public Channel(dynamic data)
        //{
        //    id = Utility.TryGetProperty(data, "id");
        //    name = Utility.TryGetProperty(data, "name");
        //    creator = Utility.TryGetProperty(data, "creator");
        //    is_archived = Utility.TryGetProperty(data, "is_archived", false);
        //    is_general = Utility.TryGetProperty(data, "is_general", false);
        //    is_member = Utility.TryGetProperty(data, "is_member", false);
        //    members = new List<string>();
        //    if (Utility.HasProperty(data, "members"))
        //    {
        //        foreach (String strMember in data.members)
        //        {
        //            members.Add(strMember);
        //        }
        //    }
        //}

        public Channel(dynamic data)
        {
            id = Utility.TryGetProperty(data, "id");
            name = Utility.TryGetProperty(data, "name");
            creator = Utility.TryGetProperty(data, "creator");
            is_archived = Utility.TryGetProperty(data, "is_archived", false);
            is_general = Utility.TryGetProperty(data, "is_general", false);
            is_member = Utility.TryGetProperty(data, "is_member", false);
            members = new List<string>();
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
