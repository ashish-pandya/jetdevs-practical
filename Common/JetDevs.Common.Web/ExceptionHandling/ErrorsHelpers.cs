using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace JetDevs.Common.Web.ExceptionHandling
{
    /// <summary>
    /// Errors Helpers
    /// </summary>
    public class ErrorsHelpers
    {
        /// <summary>
        /// Add multiple errors to model state
        /// </summary>
        /// <param name="identityResult"></param>
        /// <returns></returns>
        public static ErrorResponse GenerateErrorResponse(IdentityResult identityResult)
        {
            var errorResponse = new ErrorResponse();

            foreach (var e in identityResult.Errors)
            {
                errorResponse.Errors.Add(
                        new Error
                        {
                            FieldName = e.Code,
                            Message = e.Description
                        }
                    );
            }

            return errorResponse;
        }

        /// <summary>
        /// Add single error to model state
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static ErrorResponse GenerateErrorResponse(string code, string description)
        {
            var errorResponse = new ErrorResponse();
            errorResponse.Errors.Add(
                        new Error
                        {
                            FieldName = code,
                            Message = description
                        }
                    );
            return errorResponse;
        }

        /// <summary>
        /// Creates ErrorResponse from ModelState errors
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static ErrorResponse GenerateErrorResponse(ModelStateDictionary modelState)
        {
            var errorResponse = new ErrorResponse();

            var errorInModelState = modelState
                   .Where(x => x.Value.Errors.Count > 0)
                   .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)).ToArray();

            foreach (var error in errorInModelState)
            {
                errorResponse.Errors.Add(
                        new Error
                        {
                            FieldName = error.Key,
                            Message = string.Join(", ", error.Value)
                        }
                    );
            }
            return errorResponse;
        }
    }
}
