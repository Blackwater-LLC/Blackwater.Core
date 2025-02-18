using Microsoft.AspNetCore.Http;

namespace Blackwater.Core.Common.Exceptions.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            // preferably it would be nice to have a traceid or some trace uuid to return to later on diagnose what actually went wrong
            // for now it will just serve as a fail safe to not return stack traces and stuff
            var result = new { message = "An error occurred." };
            return context.Response.WriteAsync(result.ToString());
        }
    }

}
