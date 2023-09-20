using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class UnauthorizedResponseMiddleware
{
    private readonly RequestDelegate _next;

    public UnauthorizedResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            var errorResponse = new { message = "Unauthorized: Bạn cần đăng nhập để thực hiện" };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}

public static class UnauthorizedResponseMiddlewareExtensions
{
    public static IApplicationBuilder UseUnauthorizedResponse(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UnauthorizedResponseMiddleware>();
    }
}
