using Food_Market_BE.Shared.Middleware;

namespace Food_Market_BE.Shared.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<RequestLoggingMiddleware>();

            return app;
        }
    }
}