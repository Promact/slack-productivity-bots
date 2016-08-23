using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.Bot
{

    public class MetaData
    {
        public List<BotDetails> botsDetails;
        public List<Channel> channels;
        public List<Group> groups;
        public bool ok;
        public string url;
        public List<Users> users = null;


        public MetaData(dynamic Message)
        {
            if (Utility.HasProperty(Message, "bots"))
            {
                BotDetails rtmBot;
                foreach (dynamic bot in Message.bots)
                {
                    rtmBot = new BotDetails();
                    rtmBot.Deleted = Utility.TryGetProperty(bot, "deleted", false);
                    rtmBot.Id = Utility.TryGetProperty(bot, "id", "");
                    rtmBot.Name = Utility.TryGetProperty(bot, "name", "");
                    botsDetails.Add(rtmBot);
                }
            }
            if (Utility.HasProperty(Message, "channels"))
            {
                Channel rtmChannel;
                foreach (dynamic channel in Message.channels)
                {
                    rtmChannel = new Channel(this);
                    rtmChannel.creator = channel.creator;
                    rtmChannel.has_pins = channel.has_pins;
                    rtmChannel.id = channel.id;
                    rtmChannel.is_archived = channel.is_archived;
                    rtmChannel.is_channel = channel.is_channel;
                    rtmChannel.is_general = channel.is_general;
                    rtmChannel.is_member = channel.is_member;
                    rtmChannel.name = channel.name;
                    this.channels.Add(rtmChannel);
                }
            }
            if (Utility.HasProperty(Message, "groups"))
            {
                Group rtmGroup;
                foreach (dynamic channel in Message.groups)
                {
                    rtmGroup = new Group(this);
                    rtmGroup.creator = channel.creator;
                    rtmGroup.id = channel.id;
                    rtmGroup.is_open = channel.is_open;
                    rtmGroup.name = channel.name;
                    this.groups.Add(rtmGroup);
                }
            }
            
            url = Message.url;
            if (Utility.HasProperty(Message, "users"))
            {
                Users rtmUser;
                foreach (dynamic user in Message.users)
                {
                    rtmUser = new Users();
                    rtmUser.ID = Utility.TryGetProperty(user, "id");
                    rtmUser.Name = Utility.TryGetProperty(user, "name");
                    rtmUser.Presence = Utility.TryGetProperty(user, "presence");
                    rtmUser.Profile = new Dictionary<string, string>();
                    rtmUser.TeamId = Utility.TryGetProperty(user, "team_id");
                    users.Add(rtmUser);
                }
            }
        }
    }

}
