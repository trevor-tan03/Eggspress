using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace backend.Filters;

public class BoxAuthAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _routeParam;

    public BoxAuthAttribute(string routeParam)
    {
        _routeParam = routeParam;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var codeObjExists = context.RouteData.Values.TryGetValue(_routeParam, out var codeObj);
        if (!codeObjExists || codeObj == null)
        {
            context.Result = new BadRequestObjectResult("Box code is missing.");
            return;
        }

        string code = codeObj.ToString()!;
        var cookie = $"box_auth_{code}";
        var cookies = context.HttpContext.Request.Cookies;

        var boxCookieExists = cookies.TryGetValue(cookie, out var boxCookieValue);
        if (!boxCookieExists || boxCookieValue != "true")
        {
            context.Result = new UnauthorizedObjectResult("Unauthorized: Password required.");
            return;
        }

        await next();
    }
}