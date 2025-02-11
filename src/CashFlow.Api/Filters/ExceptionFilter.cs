using CashFlow.Communication.Responses;
using CashFlow.Exception;
using CashFlow.Exception.ExceptionsBase;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CashFlow.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
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
            var errorResponse = new ResponseError(ResourceErrorMessages.UNKNOWN_ERROR);

            context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Result = new ObjectResult(errorResponse);
        }
    }
}
