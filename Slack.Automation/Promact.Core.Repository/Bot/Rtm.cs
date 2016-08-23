using Promact.Erp.DomainModel.ApplicationClass;
using Promact.Erp.DomainModel.ApplicationClass.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.Bot
{
    public partial class Rtm
    {
        public class MetaData
        {


            public List<BotDetails> bots;
            public List<Channel> channels;
            public List<Group> groups;
            public bool ok;
            public string url;
            public List<Users> users;

            //public MetaData(dynamic Message)
            //{
            //    try
            //    {
            //        bots = new List<BotDetails>();
            //        if (Utility.HasProperty(Message, "bots"))
            //        {
            //            BotDetails rtmBot;
            //            foreach (dynamic bot in Message.bots)
            //            {
            //                rtmBot = new BotDetails();
            //                rtmBot.Deleted = Utility.TryGetProperty(bot, "deleted", false);
            //                rtmBot.Id = Utility.TryGetProperty(bot, "id", "");
            //                rtmBot.Name = Utility.TryGetProperty(bot, "name", "");
            //                bots.Add(rtmBot);
            //            }
            //        }

            //        channels = new List<Channel>();
            //        if (Utility.HasProperty(Message, "channels"))
            //        {
            //            Channel rtmChannel;

            //            foreach (dynamic channel in Message.channels)
            //            {
            //                rtmChannel = new Channel(this);
            //                rtmChannel.creator = channel.creator;
            //                rtmChannel.has_pins = channel.has_pins;
            //                rtmChannel.id = channel.id;
            //                rtmChannel.is_archived = channel.is_archived;
            //                rtmChannel.is_channel = channel.is_channel;
            //                rtmChannel.is_general = channel.is_general;
            //                rtmChannel.is_member = channel.is_member;
            //                rtmChannel.name = channel.name;
            //                channels.Add(rtmChannel);
            //            }
            //        }

            //        groups = new List<Group>();
            //        if (Utility.HasProperty(Message, "groups"))
            //        {
            //            Group rtmGroup;
            //            foreach (dynamic channel in Message.groups)
            //            {
            //                rtmGroup = new Group(this);
            //                rtmGroup.creator = channel.creator;
            //                rtmGroup.id = channel.id;
            //                rtmGroup.name = channel.name;
            //                groups.Add(rtmGroup);
            //            }
            //        }
            //        this.url = Message.url;

            //        users = new List<Users>();
            //        if (Utility.HasProperty(Message, "users"))
            //        {
            //            Users rtmUser;
            //            foreach (dynamic user in Message.users)
            //            {
            //                rtmUser = new Users();
            //                rtmUser.ID = Utility.TryGetProperty(user, "id");
            //                rtmUser.Name = Utility.TryGetProperty(user, "name");
            //                rtmUser.Presence = Utility.TryGetProperty(user, "presence");
            //                rtmUser.Profile = new Dictionary<string, string>();
            //                rtmUser.TeamId = Utility.TryGetProperty(user, "team_id");
            //                this.users.Add(rtmUser);
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //        throw ex;
            //    }
            //}



            public MetaData(dynamic Message)
            {

                try
                {
                    bots = new List<BotDetails>();
                    BotDetails rtmBot;
                    foreach (dynamic bot in Message.bots)
                    {
                        rtmBot = new BotDetails();
                        rtmBot.Deleted = bot.deleted;
                        rtmBot.Id = bot.id;
                        rtmBot.Name = bot.name;
                        bots.Add(rtmBot);
                    }

                    channels = new List<Channel>();
                    Channel rtmChannel;
                    foreach (dynamic channel in Message.channels)
                    {
                        // rtmChannel = new Channel(this);
                        rtmChannel = new Channel();
                        rtmChannel.creator = channel.creator;
                        rtmChannel.has_pins = channel.has_pins;
                        rtmChannel.id = channel.id;
                        rtmChannel.is_archived = channel.is_archived;
                        rtmChannel.is_channel = channel.is_channel;
                        rtmChannel.is_general = channel.is_general;
                        rtmChannel.is_member = channel.is_member;
                        rtmChannel.name = channel.name;
                        channels.Add(rtmChannel);
                    }

                    groups = new List<Group>();
                    Group rtmGroup;
                    foreach (dynamic group in Message.groups)
                    {
                        //rtmGroup = new Group(this);
                        rtmGroup = new Group();
                        rtmGroup.creator = group.creator;
                        rtmGroup.id = group.id;
                        rtmGroup.name = group.name;
                        rtmGroup.members = new List<string>();
                        foreach (var strMember in group.members)
                        {
                            rtmGroup.members.Add((string)strMember);
                        }
                        groups.Add(rtmGroup);
                    }
                    this.url = Message.url;

                    users = new List<Users>();
                    Users rtmUser;
                    foreach (dynamic user in Message.users)
                    {
                        rtmUser = new Users();
                        rtmUser.ID = user.id;
                        rtmUser.Name = user.name;
                        rtmUser.Presence = user.presence;
                        rtmUser.Profile = new Dictionary<string, string>();
                        rtmUser.TeamId = user.team_id;
                        this.users.Add(rtmUser);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
