using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using MyBudget.BLL.Exceptions;

namespace MyBudget.JWT.Middlewares;

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
            _logger.LogError(ex, "Unhandled exception occurred while processing request: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Not Found", exception.Message),
            ConflictException => (HttpStatusCode.Conflict, "Conflict", exception.Message),
            ArgumentException => (HttpStatusCode.BadRequest, "Invalid Argument", exception.Message),
            ValidationException => (HttpStatusCode.BadRequest, "Validation Error", exception.Message),
            JwtTokenMissingException => (HttpStatusCode.Unauthorized, "Token Missing", exception.Message),
            JwtTokenInvalidException => (HttpStatusCode.Unauthorized, "Invalid Token", exception.Message),
            JwtTokenExpiredException => (HttpStatusCode.Unauthorized, "Token Expired", exception.Message),
            JwtUnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized", "Unauthorized access. Please login."),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Unauthorized", "Unauthorized access. Please login."),
            _ => (HttpStatusCode.InternalServerError, "Server Error", "An unexpected error occurred.")
        };

        var problemDetails = new
        {
            status = (int)statusCode,
            title,
            detail,
            instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}