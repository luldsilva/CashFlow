using CashFlow.Communication.Responses;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace CashFlow.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if(context.Exception is CashFlowException)
            {
                HandleProjectException(context);
            }
            else
            {
                ThrowUnknownError(context);
            }
        }

        public void HandleProjectException(ExceptionContext context) 
        {
            var cashFlowException = context.Exception as CashFlowException;

            var errorResponse = new ResponseError(cashFlowException!.GetErrors());

            context.HttpContext.Response.StatusCode = cashFlowException.StatusCode;

            context.Result = new ObjectResult(errorResponse);
        }

        public void ThrowUnknownError(ExceptionContext context) 
        {
            _logger.LogError(context.Exception, "Unhandled exception processing request {Method} {Path}",
                context.HttpContext.Request.Method,
                context.HttpContext.Request.Path);

            var errorResponse = new ResponseError(ResourceErrorMessages.UNKNOWN_ERROR);

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(errorResponse);
        }
    }
}
