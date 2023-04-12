using Innoloft.Shared.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Innoloft.Web.Helpers;

public class StructuredResponseActionFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        IActionResult result = context.Result;

        if (result is ObjectResult objectResult)
        {
            int statusCode = objectResult.StatusCode ?? StatusCodes.Status200OK;

            if (statusCode == StatusCodes.Status404NotFound)
            {
                context.Result = new NotFoundObjectResult(Response<object>.Failure("Resource not found."));
            }
            else if (statusCode == StatusCodes.Status400BadRequest)
            {
                context.Result = new BadRequestObjectResult(Response<object>.Failure(objectResult.Value.ToString()));
            }
            else if (statusCode >= 400)
            {
                context.Result = new ObjectResult(Response<object>.Failure(objectResult.Value.ToString()))
                {
                    StatusCode = statusCode
                };
            }
            else
            {
                context.Result = new OkObjectResult(Response<object>.Success(objectResult.Value));
            }

            context.HttpContext.Response.StatusCode = statusCode;
        }

        await next();
    }
}