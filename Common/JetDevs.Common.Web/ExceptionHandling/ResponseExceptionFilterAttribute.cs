using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace JetDevs.Common.Web.ExceptionHandling
{
    /// <summary>
    /// This filter is intended to respond with BadRequestObjectResult in case of unhandled exception.
    /// Model state will contain code and message.
    /// Default code is: exception-handling-request
    /// Default message is: There was a problem completing your request. Please try again, our engineers have been notified
    /// It cab be added either as global filter for the service or as an attribute to the service method.
    /// Example 1 :
    /// <code>
    /// services.AddControllers(
    ///        options =>
    ///            {    
    ///                options.Filters.Add(typeof(ResponseExceptionFilterAttribute));
    ///            });
    /// </code>
    /// Example 2 :
    /// ...
    /// <code>
    /// [ResponseExceptionFilterAttribute("company-exists", "Company already exists.")]
    /// public ActionResult CreateCompany([FromBody]Company model)
    /// </code>
    /// </summary>
    public class ResponseExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly string _code = "exception-handling-request";
        private readonly string _message = "There was a problem completing your request. Please try again, our engineers have been notified";

        /// <summary>
        /// Default constructor. Used for global filter or instantiation without the parameters.
        /// </summary>
        public ResponseExceptionFilterAttribute()
        {
        }

        /// <summary>
        /// Intstantiate a new instance of the attribute with custom code and message
        /// </summary>
        /// <param name="code">The error code</param>
        /// <param name="message">The error message</param>
        public ResponseExceptionFilterAttribute(string code,
            string message)
        {
            _code = code;
            _message = message;
        }

        /// <summary>
        /// Executed on unhandled exception processing
        /// </summary>
        /// <param name="context">Exception context</param>
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is ValidationFailedException)
            {
                var ex = (context.Exception as ValidationFailedException);
                var modelState = ErrorsHelpers.GenerateErrorResponse(ex.Code, ex.Message);
                context.ExceptionHandled = true;
                context.Result = new BadRequestObjectResult(modelState);
            }
            if (context.ModelState.ErrorCount == 0)
            {
                context.Result = new BadRequestObjectResult(ErrorsHelpers.GenerateErrorResponse(_code, _message));
            }

        }
    }
}
