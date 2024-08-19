using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace WorkBid.Authorization
{
    public class AdminLoggedAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var adminSession = session.GetString("LoggedAdmin");

            if (string.IsNullOrEmpty(adminSession) || !int.TryParse(adminSession, out int adminId))
            {
                context.Result = new RedirectToActionResult("Index", "Admin", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
