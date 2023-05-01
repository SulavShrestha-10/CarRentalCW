using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalApp.Controllers
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new ChallengeResult();
            }
            else if (!string.IsNullOrEmpty(Roles) && !filterContext.HttpContext.User.IsInRole(Roles))
            {
                string[] roles = Roles.Split(',');
                if (!roles.Any(role => filterContext.HttpContext.User.IsInRole(role)))
                {
                    HandleUnauthorizedRequest(filterContext);
                }
            }
        }

        protected void HandleUnauthorizedRequest(AuthorizationFilterContext filterContext)
        {
            filterContext.Result = new RedirectToActionResult("Error", "Home", null);
        }
    }
}
