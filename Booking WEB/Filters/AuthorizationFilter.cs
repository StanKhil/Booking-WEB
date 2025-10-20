using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Booking_WEB.Filters
{
    public class AuthorizationFilter : ActionFilterAttribute
    {
        override public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine(context.HttpContext.Request.Method == "GET");
            if ((context.HttpContext.User.Identity?.IsAuthenticated ?? false )
                || context.HttpContext.Request.Method == "GET"
                )
            {
                
                base.OnActionExecuting(context);
            }
            else
            {
                context.Result = new JsonResult(new
                {
                    status = "401",
                    message = "Unauthorized"
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
        }
    }
}
