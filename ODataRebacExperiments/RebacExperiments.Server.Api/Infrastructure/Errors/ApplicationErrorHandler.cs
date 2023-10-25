// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.Options;
using Microsoft.OData;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Infrastructure.OData;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace RebacExperiments.Server.Api.Infrastructure.Errors
{

    /// <summary>
    /// Handles errors returned by the application.
    /// </summary>
    public class ApplicationErrorHandler
    {
        private readonly ILogger<ApplicationErrorHandler> _logger;

        private readonly ApplicationErrorHandlerOptions _options;

        public ApplicationErrorHandler(ILogger<ApplicationErrorHandler> logger, IOptions<ApplicationErrorHandlerOptions> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        public ActionResult HandleInvalidModelState(HttpContext httpContext, ModelStateDictionary modelStateDictionary)
        {
            _logger.TraceMethodEntry();

            ODataError error = new ODataError()
            {
                ErrorCode = ErrorCodes.ValidationFailed,
                Message = "One or more validation errors occured",
                Details = GetODataErrorDetails(modelStateDictionary),

            };

            // If there was an exception, use this as ODataInnerError
            if (TryGetFirstException(modelStateDictionary, out var exception))
            {
                AddInnerError(httpContext, error, exception);
            }

            return new BadRequestObjectResult(error);
        }

        private bool TryGetFirstException(ModelStateDictionary modelStateDictionary, [NotNullWhen(true)] out Exception? e)
        {
            e = null;

            foreach (var modelStateEntry in modelStateDictionary)
            {
                foreach (var modelError in modelStateEntry.Value.Errors)
                {
                    if (modelError.Exception != null)
                    {
                        e = modelError.Exception;

                        return true;
                    }
                }
            }

            return false;
        }

        private List<ODataErrorDetail> GetODataErrorDetails(ModelStateDictionary modelStateDictionary)
        {
            _logger.TraceMethodEntry();

            // Validation Details
            var result = new List<ODataErrorDetail>();

            foreach (var modelStateEntry in modelStateDictionary)
            {
                foreach (var modelError in modelStateEntry.Value.Errors)
                {
                    // We cannot make anything sensible for the caller here. We log it, but then go on 
                    // as if nothing has happened. Alternative is to populate a chain of ODataInnerError 
                    // or abuse the ODataErrorDetails...
                    if (modelError.Exception != null)
                    {
                        _logger.LogError(modelError.Exception, "Invalid ModelState due to an exception");

                        continue;
                    }

                    var odataErrorDetail = new ODataErrorDetail
                    {
                        ErrorCode = ErrorCodes.ValidationFailed,
                        Message = modelError.ErrorMessage,
                        Target = modelStateEntry.Key,
                    };

                    result.Add(odataErrorDetail);
                }
            }

            return result;
        }

        public ActionResult HandleException(HttpContext httpContext, Exception exception)
        {
            _logger.TraceMethodEntry();

            _logger.LogError(exception, "Call to '{RequestPath}' failed due to an Exception", httpContext.Request.Path);

            return exception switch
            {
                AuthenticationFailedException e => HandleAuthenticationException(httpContext, e),
                EntityConcurrencyException e => HandleEntityConcurrencyException(httpContext, e),
                EntityNotFoundException e => HandleEntityNotFoundException(httpContext, e),
                EntityUnauthorizedAccessException e => HandleEntityUnauthorizedException(httpContext, e),
                Exception e => HandleSystemException(httpContext, e)
            };
        }

        private ActionResult HandleAuthenticationException(HttpContext httpContext, AuthenticationFailedException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.AuthenticationFailed,
                Message = e.ErrorMessage,
            };

            AddInnerError(httpContext, error, e);

            return new UnauthorizedODataResult(error);
        }

        private ActionResult HandleEntityConcurrencyException(HttpContext httpContext, EntityConcurrencyException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = e.ErrorCode,
                Message = e.ErrorMessage,
            };

            AddInnerError(httpContext, error, e);

            return new ConflictODataResult(error);
        }

        private ActionResult HandleEntityNotFoundException(HttpContext httpContext, EntityNotFoundException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = e.ErrorCode,
                Message = e.ErrorMessage,
            };

            AddInnerError(httpContext, error, e);

            return new NotFoundODataResult(error);
        }

        private ActionResult HandleEntityUnauthorizedException(HttpContext httpContext, EntityUnauthorizedAccessException e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = e.ErrorCode,
                Message = e.ErrorMessage,
            };

            AddInnerError(httpContext, error, e);

            return new ForbiddenODataResult(error);
        }

        private ActionResult HandleSystemException(HttpContext httpContext, Exception e)
        {
            _logger.TraceMethodEntry();

            var error = new ODataError
            {
                ErrorCode = ErrorCodes.InternalServerError,
                Message = "An Internal Server Error occured"
            };

            AddInnerError(httpContext, error, e);

            return new ObjectResult(error)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
        }

        private void AddInnerError(HttpContext httpContext, ODataError error, Exception e)
        {
            _logger.TraceMethodEntry();

            error.InnerError = new ODataInnerError();

            error.InnerError.Properties["trace-id"] = new ODataPrimitiveValue(httpContext.TraceIdentifier);

            if (_options.IncludeExceptionDetails)
            {
                error.InnerError.Message = e.Message;
                error.InnerError.StackTrace = e.StackTrace;
                error.InnerError.TypeName = e.GetType().Name;
            }
        }
    }
}