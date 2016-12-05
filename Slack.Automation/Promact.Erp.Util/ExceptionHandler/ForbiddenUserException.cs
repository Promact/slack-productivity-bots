using System;

namespace Promact.Erp.Util.ExceptionHandler
{
    public class ForbiddenUserException : Exception
    {
        /// <summary>
        ///  Initializes a new instance of the ForbiddenUserException class.
        /// </summary>
        public ForbiddenUserException()
        {
        }


        /// <summary>
        ///    Initializes a new instance of the ForbiddenUserException class with a specified error message.
        /// </summary>
        /// <param name="message">error message</param>
        public ForbiddenUserException(string message)
        : base(message)
        {
        }


        /// <summary>
        /// Initializes a new instance of the ForbiddenUserException class with a specified error message and a reference to the inner exception that is the cause of this exception.        
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ForbiddenUserException(string message, Exception innerException)
        : base(message, innerException)
        {
        }
    }
}



