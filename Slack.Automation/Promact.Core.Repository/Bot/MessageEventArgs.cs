using Promact.Core.Repository.Client;
using Promact.Erp.DomainModel.ApplicationClass.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Core.Repository.Bot
{
    public class MessageEventArgs
    {
        private BotClient _client;

        private string _channel;
        private string _user;
        private string _text;
        private string _team;


        public MessageEventArgs(BotClient client, dynamic data)
        {
            _client = client;
            _channel = data.channel;
            _user = data.user;
            _text = data.text;
            _team = data.team;
        }

        public string Text
        {
            get
            {
                return _text;
            }
        }

        public string User
        {
            get
            { 
                return _user;
            }
        }

        public Users UserInfo
        {
            get
            {
                foreach (Users user in _client.MetaData.users)
                {
                    if (user.ID == _user)
                    {
                        return user;
                    }
                }
                return null;
            }
        }

        public Group GroupInfo
        {
            get
            {
                foreach (Group group in _client.MetaData.groups)
                {
                    if (group.id == _channel)
                    {
                        return group;
                    }
                }
                return null;
            }
        }

    }
}
