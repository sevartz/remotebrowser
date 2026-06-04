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
        catch (Exception ex)
        {
            _logger.LogError(ex, "unhandled exception {path}", context.Request.Path);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new {error = "server error"});
        }
    }

}

public class TimeResponse
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TimeResponse> _logger;

    public TimeResponse(RequestDelegate next, ILogger<TimeResponse> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var start = DateTime.UtcNow;
        _logger.LogInformation("→ handling request {method} {path}", context.Request.Method, context.Request.Path);
        await _next(context);
        var end = DateTime.UtcNow;
        _logger.LogInformation("← request completed {method} {path} in {duration}ms", context.Request.Method, context.Request.Path, (end - start).TotalMilliseconds);
    }
}