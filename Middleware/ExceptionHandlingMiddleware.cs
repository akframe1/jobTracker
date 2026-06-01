using System.Text.Json;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ApplicationNotFoundException ex)
        {
            _logger.LogWarning(ex, "Application not found: ID {Id}", ex.Id);
            await WriteResponse(context, StatusCodes.Status404NotFound, "NOT_FOUND", ex.Message);
        }
        catch (InvalidStatusTransitionException ex)
        {
            _logger.LogWarning(ex, "Invalid status transition: {From} -> {To}", ex.FromStatus, ex.ToStatus);
            await WriteResponse(context, StatusCodes.Status400BadRequest, "INVALID_STATUS_TRANSITION", ex.Message);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failed: {Errors}", string.Join(", ", ex.Errors));
            await WriteResponse(context, StatusCodes.Status400BadRequest, "VALIDATION_ERROR", ex.Message, ex.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await WriteResponse(context, StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponse(
        HttpContext context,
        int statusCode,
        string errorCode,
        string message,
        IEnumerable<string>? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var body = new
        {
            errorCode,
            message,
            errors = errors ?? Enumerable.Empty<string>()
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}