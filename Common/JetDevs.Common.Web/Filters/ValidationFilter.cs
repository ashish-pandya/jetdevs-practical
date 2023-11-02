using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using JetDevs.Common.Web.ExceptionHandling;
using System.Threading.Tasks;

namespace JetDevs.Common.Web.Filters
{
    /// <summary>
    /// Validation Filter
    /// </summary>
    public class ValidationFilter : IAsyncActionFilter
    {
        /// <summary>
        /// Validate the request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(ErrorsHelpers.GenerateErrorResponse(context.ModelState));
                return;
            }

            await next();
        }
    }
}
