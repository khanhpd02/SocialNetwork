using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SocialNetwork.DTO;

namespace SocialNetwork.Middlewares
{
    public class ResponseFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                if (context.HttpContext.Response.StatusCode >= 200 && context.HttpContext.Response.StatusCode < 300)
                {
                    if (objectResult.Value is PaginationData paginationData)
                    {
                        objectResult.Value = new PagedResponse
                        {
                            Data = paginationData.Data,
                            Success = true,
                            Message = "",
                            pagable = paginationData.pagable
                        };
                    }
                    else
                    {
                        objectResult.Value = new ReponseModel
                        {
                            Data = objectResult.Value,
                            Success = true,
                            Message = ""
                        };
                    }
                }
                else
                {
                    objectResult.Value = new ReponseModel
                    {
                        Data = null,
                        Success = false,
                        Message = objectResult.Value?.ToString()
                    };
                }
            }
        }
    }
}
