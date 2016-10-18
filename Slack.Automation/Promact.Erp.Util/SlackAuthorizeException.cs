using System;

namespace Promact.Erp.Util
{
    public class SlackAuthorizeException : Exception
    {
        public SlackAuthorizeException()
        {
        }

        public SlackAuthorizeException(string message)
        : base(message)
        {
        }

        public SlackAuthorizeException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
