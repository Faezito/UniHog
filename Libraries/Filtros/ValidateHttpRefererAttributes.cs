using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UniHog.Libraries.Filtros
{
    public class ValidateHttpRefererAttributes : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Home", null);
                return;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //Possivel Logging
        }
    }
}