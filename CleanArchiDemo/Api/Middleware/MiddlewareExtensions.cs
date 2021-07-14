using Microsoft.AspNetCore.Builder;

namespace Api.Middleware
{
    public static class MiddlewareExtensions
    {
        // Called in the startup.cs  ( Configure() )
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
