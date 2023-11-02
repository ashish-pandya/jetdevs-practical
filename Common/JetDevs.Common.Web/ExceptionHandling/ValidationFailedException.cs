using System;

namespace JetDevs.Common.Web.ExceptionHandling
{
    /// <summary>
    /// Validation Failed Exception
    /// </summary>
    public class ValidationFailedException : Exception
    {
        /// <summary>
        /// Code/Key
        /// </summary>
        public string Code { get; private set; }

        /// <summary>
        /// Creates instance of ValidationFailedException
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        public ValidationFailedException(string code, string message) : base(message)
        {
            Code = code;
        }
    }
}
