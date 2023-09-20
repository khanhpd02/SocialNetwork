using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class ForbiddenResponseMiddleware
{
    private readonly RequestDelegate _next;

    public ForbiddenResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            var errorResponse = new { message = "Access denied: Không có quyền Truy Cập" };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}

public static class ForbiddenResponseMiddlewareExtensions
{
    public static IApplicationBuilder UseForbiddenResponse(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ForbiddenResponseMiddleware>();
    }
}
