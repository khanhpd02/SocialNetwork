using Microsoft.AspNetCore.Mvc.Filters;
using SocialNetwork.ExceptionModel;

namespace SocialNetwork.Middlewares
{
    public class AuthenticationFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity!.IsAuthenticated)
            {
                throw new InvalidTokenException();
            }
            return;
        }
    }
}
