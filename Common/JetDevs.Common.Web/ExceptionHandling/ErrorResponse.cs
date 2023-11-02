using System.Collections.Generic;

namespace JetDevs.Common.Web.ExceptionHandling
{
    /// <summary>
    /// Response object in case of Error or Validation error
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// List of errors
        /// </summary>
        public IList<Error> Errors { get; set; } = new List<Error>();
    }
}
