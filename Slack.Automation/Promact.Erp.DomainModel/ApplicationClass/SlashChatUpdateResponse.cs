using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashChatUpdateResponse
    {
        /// <summary>
        /// Action details of Slash ChatUpdateMethod
        /// </summary>
        public SlashChatUpdateResponseAction actions { get; set; }

        /// <summary>
        /// Callback_id details of Slash ChatUpdateMethod
        /// </summary>
        public int callback_id { get; set; }

        /// <summary>
        /// Team details of Slash ChatUpdateMethod
        /// </summary>
        public SlashChatUpdateResponseTeam team { get; set; }

        /// <summary>
        /// Channel details of Slash ChatUpdateMethod
        /// </summary>
        public SlashChatUpdateResponseChannelUser channel { get; set; }

        /// <summary>
        /// User details of Slash ChatUpdateMethod
        /// </summary>
        public SlashChatUpdateResponseChannelUser user { get; set; }

        /// <summary>
        /// action_ts of Slash ChatUpdateMethod
        /// </summary>
        public string action_ts { get; set; }

        /// <summary>
        /// message_ts of Slash ChatUpdateMethod
        /// </summary>
        public string message_ts { get; set; }

        /// <summary>
        /// token of Slash ChatUpdateMethod
        /// </summary>
        public string token { get; set; }
    }
}
