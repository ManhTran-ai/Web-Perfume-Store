using System.Net;
using System.Text.Json;

namespace GuhaStore.Web.Middleware;

public class DatabaseErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DatabaseErrorHandlerMiddleware> _logger;

    public DatabaseErrorHandlerMiddleware(RequestDelegate next, ILogger<DatabaseErrorHandlerMiddleware> logger)
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
            _logger.LogError(ex, "An error occurred processing the request");
            
            // Check if it's a database connection error
            if (ex.Message.Contains("Unable to connect") || 
                ex.Message.Contains("Access denied") ||
                ex.Message.Contains("Unknown database"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                context.Response.ContentType = "text/html; charset=utf-8";
                
                await context.Response.WriteAsync(@"
                    <html>
                        <head><title>Database Connection Error</title></head>
                        <body>
                            <h1>Database Connection Error</h1>
                            <p>Unable to connect to the database. Please check:</p>
                            <ul>
                                <li>MySQL server is running</li>
                                <li>Database 'dbperfume' exists</li>
                                <li>Connection string in appsettings.json is correct</li>
                                <li>User credentials are correct</li>
                            </ul>
                            <p><a href='/'>Return to Home</a></p>
                        </body>
                    </html>");
                return;
            }
            
            // Re-throw other exceptions to be handled by the default error handler
            throw;
        }
    }
}

