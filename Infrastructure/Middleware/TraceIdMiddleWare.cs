using Microsoft.AspNetCore.Http;

public class TraceIdMiddleware
{
    private readonly RequestDelegate _next;

    public TraceIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

   
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Items.ContainsKey("TraceId"))
            context.Items["TraceId"] = Guid.NewGuid().ToString();

        await _next(context);
    }
}
