using System.Text.Json;
using Food_Market_BE.Shared.Exceptions;
using Food_Market_BE.Shared.Responses;

namespace Food_Market_BE.Shared.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";

                int statusCode = 500;
                string message = "Internal server error";
                object errors = null;

                if (ex is AppException appEx)
                {
                    statusCode = appEx.StatusCode;
                    message = appEx.Message;
                    errors = appEx.Errors;
                }

                context.Response.StatusCode = statusCode;

                var response = ApiResponse<object>.FailResponse(message, errors);

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}