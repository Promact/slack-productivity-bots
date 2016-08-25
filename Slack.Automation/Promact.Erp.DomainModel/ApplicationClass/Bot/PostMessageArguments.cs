using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.Bot
{
    public class PostMessageArguments
    {
        public string Channel { get; set; }

        public string Text { get; set; }

        public string Username { get; set; }







        public Boolean as_user { get; set; }
        public String parse { get; set}
        public Int32 link_names { get; set; }

        public Boolean unfurl_links { get; set; }
        public Boolean unfurl_media { get; set; }
        public String icon_url { get; set; }
        public String icon_emoji { get; set; }

    }

}
