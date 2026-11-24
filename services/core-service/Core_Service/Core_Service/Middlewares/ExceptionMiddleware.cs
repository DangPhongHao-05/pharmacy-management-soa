// Middlewares/ExceptionMiddleware.cs
using System.Net;
using System.Text.Json;

namespace Core_Service.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Phân loại mã lỗi HTTP tương ứng với từng loại Exception
            var statusCode = exception switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,           // 404
                InvalidOperationException => (int)HttpStatusCode.BadRequest,   // 400
                ArgumentException => (int)HttpStatusCode.BadRequest,           // 400
                _ => (int)HttpStatusCode.InternalServerError                   // 500
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                statusCode = statusCode,
                message = exception.Message
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(json);
        }
    }
}