using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass
{
    public class SlashChatUpdateResponse
    {
        public SlashChatUpdateResponseAction actions { get; set; }
        public int callback_id { get; set; }
        public SlashChatUpdateResponseTeam team { get; set; }
        public SlashChatUpdateResponseChannelUser channel { get; set; }
        public SlashChatUpdateResponseChannelUser user { get; set; }
        public string action_ts { get; set; }
        public string message_ts { get; set; }
        public string token { get; set; }
    }
}
