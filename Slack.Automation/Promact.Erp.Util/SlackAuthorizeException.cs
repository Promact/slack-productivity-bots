using System;

namespace Promact.Erp.Util
{
    public class SlackAuthorizeException : Exception
    {
        /// <summary>
        ///  Initializes a new instance of the SlackAuthorizeException class.
        /// </summary>
        public SlackAuthorizeException()
        {
        }

        /// <summary>
        ///    Initializes a new instance of the SlackAuthorizeException class with a specified error message.
        /// </summary>
        /// <param name="message">error message</param>
        public SlackAuthorizeException(string message)
        : base(message)
        {
        }


        /// <summary>
        /// Initializes a new instance of the SlackAuthorizeException class with a specified error message and a reference to the inner exception that is the cause of this exception.        
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SlackAuthorizeException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
