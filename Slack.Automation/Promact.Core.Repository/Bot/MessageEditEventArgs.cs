using Promact.Core.Repository.Client;
using Promact.Erp.DomainModel.ApplicationClass.Bot;


namespace Promact.Core.Repository.Bot
{
    public class MessageEditEventArgs
    {
        private BotClient _client;

        private Message _message;
        private string _subtype;
        private bool _hidden;
        private string _channel;
        //private Previous_Message _previous_message;
        private string _event_ts;



        public MessageEditEventArgs(BotClient client, dynamic Data)
        {
            _client = client;
            _channel = Data.channel;
            _event_ts = Data.event_ts;
            _hidden = Data.hidden;
            //if (Utility.HasProperty(Data, "message"))
            //{
            _message = new Message(Data.message);
            // }
            //_previous_message = new Previous_Message(Data.previous_message);
            _subtype = Data.subtype;

        }
    }
}
