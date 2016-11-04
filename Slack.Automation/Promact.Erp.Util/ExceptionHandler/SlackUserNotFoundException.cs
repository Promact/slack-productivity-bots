using System;

namespace Promact.Erp.Util.ExceptionHandler
{
    public class SlackUserNotFoundException :Exception
    {
        /// <summary>
        ///  Initializes a new instance of the SlackUserNotFound class.
        /// </summary>
        public SlackUserNotFoundException()
        {
        }

        /// <summary>
        ///    Initializes a new instance of the SlackUserNotFound class with a specified error message.
        /// </summary>
        /// <param name="message">error message</param>
        public SlackUserNotFoundException(string message)
        : base(message)
        {
        }


        /// <summary>
        /// Initializes a new instance of the SlackUserNotFound class with a specified error message and a reference to the inner exception that is the cause of this exception.        
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public SlackUserNotFoundException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}
