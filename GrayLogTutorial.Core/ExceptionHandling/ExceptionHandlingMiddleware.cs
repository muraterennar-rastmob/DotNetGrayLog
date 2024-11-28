using GrayLogTutorial.Core.Logs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;

namespace GrayLogTutorial.Core.ExceptionHandling;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _loggerService;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerService loggerService)
    {
        _next = next;
        _loggerService = loggerService;
    }

    public async Task Invoke(HttpContext context)
    {
        Log.Information("Request started at {Time}", DateTime.UtcNow);

        try
        {
            // Asenkron işlemse, bekleyelim
            await _next(context);


            // Senkron işleme sonrası yapılacak işlemler
            //Log.Information("Request processed successfully.");
            _loggerService.Information("Request finished at {Time}", DateTime.UtcNow);
        }
        catch (Exception exception)
        {
            // Hata işleme
            //Log.Fatal(exception, "An error occurred during request processing.");
            _loggerService.Error(exception, "Unhandled exception");

            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var result = new
        {
            StatusCode = context.Response.StatusCode,
            Message = $"Message : {exception.Message}",
            Detailed = exception.StackTrace
        };

        var jsonResponse = JsonConvert.SerializeObject(result);

        await context.Response.WriteAsync(jsonResponse);
    }
}