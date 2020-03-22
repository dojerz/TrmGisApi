using Microsoft.AspNetCore.Mvc.Filters;

namespace TrmGisApi.Filters.ActionFilters
{
    public class GeojsonResponseHeaderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //context.HttpContext.Response.Headers.Add("Content-Type", "application/json");
            //context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:59062");
            context.HttpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
        }
    }
}
