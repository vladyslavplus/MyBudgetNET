using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using MyBudget.BLL.Exceptions;

namespace MyBudget.API.Middlewares;

public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, errorMessage) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            ConflictException => (HttpStatusCode.Conflict, exception.Message),
            ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
            ValidationException => (HttpStatusCode.BadRequest, exception.Message),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        var response = new ErrorResponse
        {
            Code = (int)statusCode,
            Error = errorMessage
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
    
    private record ErrorResponse
    {
        public int Code { get; init; }
        public string Error { get; init; } = string.Empty;
    }
}