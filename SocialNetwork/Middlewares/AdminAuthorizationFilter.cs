using Microsoft.AspNetCore.Mvc.Filters;

namespace SocialNetwork.Middlewares
{
    public class AdminAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.IsInRole("Admin"))
            {
                throw new AccessDeniedException();
            }
            return;
        }
    }
}
