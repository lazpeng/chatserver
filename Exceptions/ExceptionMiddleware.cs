using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChatServer.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = (int)(exception switch
            {
                ChatPermissionException _ => HttpStatusCode.Forbidden,
                ChatAuthException _ => HttpStatusCode.Unauthorized,
                ChatBaseException _ => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError,
            });

            Console.WriteLine($"An exception occurred: {exception.Message}\n{exception.StackTrace}");

            var details = new
            {
                context.Response.StatusCode,
                exception.Message
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(details));
        }
    }
}
