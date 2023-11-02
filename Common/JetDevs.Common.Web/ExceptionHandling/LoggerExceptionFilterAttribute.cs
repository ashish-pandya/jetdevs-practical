using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace JetDevs.Common.Web.ExceptionHandling
{
    /// <summary>
    /// This filter is intended to send unhandled exception to the log.
    /// It should be added as global filter for the service.
    /// Example:
    /// services.AddControllers(
    ///        options =>
    ///            {    
    ///                options.Filters.Add(typeof(LoggerExceptionFilterAttribute));
    ///            });
    /// </summary>
    public class LoggerExceptionFilterAttribute : ExceptionFilterAttribute
    {

        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Creates a new instance of the service
        /// </summary>
        /// <param name="loggerFactory">Logger factory</param>
        public LoggerExceptionFilterAttribute(
            ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Executed on unhandled exception processing
        /// </summary>
        /// <param name="context">Exception context</param>
        public override void OnException(ExceptionContext context)
        {
            var logger = _loggerFactory.CreateLogger(typeof(LoggerExceptionFilterAttribute));
            logger.LogError(context.Exception, "Exception");
        }
    }
}
