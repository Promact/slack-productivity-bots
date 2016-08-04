using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashCommand
    {
        /// <summary>
        /// Token issued from slack
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// Team_id of slack
        /// </summary>
        public string team_id { get; set; }

        /// <summary>
        /// Team Doamin of slack
        /// </summary>
        public string team_domain { get; set; }

        /// <summary>
        /// Channel Id of the slack
        /// </summary>
        public string channel_id { get; set; }

        /// <summary>
        /// Channel Name of Slack
        /// </summary>
        public string channel_name { get; set; }

        /// <summary>
        /// User Id according to slack
        /// </summary>
        public string user_id { get; set; }

        /// <summary>
        /// User Name according to Slack
        /// </summary>
        public string user_name { get; set; }

        /// <summary>
        /// Slash Command issued by user
        /// </summary>
        public string command { get; set; }

        /// <summary>
        /// Text send along with /command by user
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// Url to response back for slash Command
        /// </summary>
        public string response_url { get; set; }
    }
}
