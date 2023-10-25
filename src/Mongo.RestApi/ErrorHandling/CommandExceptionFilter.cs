using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mongo.RestApi.ErrorHandling
{
    public class CommandExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is CommandException commandException)
            {
                var result = new { Error = commandException.Value };
                context.Result = new ObjectResult(result)
                {
                    StatusCode = (int)HttpStatusCode.Conflict
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
