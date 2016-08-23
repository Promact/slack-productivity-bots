using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Promact.Erp.DomainModel.ApplicationClass.Bot
{
    public class Message
    {
        private String _user;
        private String _text;
        private Edited _edited;
     

        public Message(dynamic Data)
        {
            _edited = new Edited(Data.edited);
            _text = Data.text;
            _user = Data.user;
        }

    }

    public class Edited
        {


            private string _user;
            private string _ts;


            public Edited(dynamic Data)
            {
                _ts = Data.ts;
                _user = Data.user;
            }

        }
}
