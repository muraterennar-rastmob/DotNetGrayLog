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
        _loggerService.Information($"Request started at {DateTime.UtcNow} - {context.Request.Method} {context.Request.Path}, Query: {context.Request.QueryString}", context);
        
        // Response'un orijinal Body'sini sakla
        var originalBodyStream = context.Response.Body;
        
        try
        {
            // Geçici bir kopyalama stream'i oluştur
            using var responseBodyStream = new ResponseForwardingStream(originalBodyStream);
            context.Response.Body = responseBodyStream;
            
            // Asenkron işlemse, bekleyelim
            await _next(context);
            
            // Response'u logla
            _loggerService.Information("Response: {StatusCode}, Content-Type: {ContentType}",
                context.Response.StatusCode,
                context.Response.ContentType);
            
            if (responseBodyStream.ContentLength > 0)
            {
                var responseBodyText = responseBodyStream.GetBufferedText();
                responseBodyText = FilterSensitiveData(responseBodyText);
                _loggerService.Information("Response Body: {ResponseBody}", responseBodyText);
            }
            
            _loggerService.Information("Request finished at {Time}", DateTime.UtcNow);
        }
        catch (Exception exception)
        {
            // Hata işleme
            //Log.Fatal(exception, "An error occurred during request processing.");
            _loggerService.Error(exception, "Unhandled exception");

            await HandleExceptionAsync(context, exception);
        }
        finally
        {
            // Orijinal Body'yi geri yükle
            context.Response.Body = originalBodyStream;
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
    
    private string FilterSensitiveData(string responseBody)
    {
        // Örnek: Şifre veya hassas bilgileri maskele
        // responseBody = Regex.Replace(responseBody, "\"password\":\".*?\"", "\"password\":\"***\"");
        return responseBody; // Gerekirse değiştirin
    }
    
    private class ResponseForwardingStream : Stream
    {
        private readonly Stream _originalStream;
        private readonly MemoryStream _bufferStream = new MemoryStream();

        public ResponseForwardingStream(Stream originalStream)
        {
            _originalStream = originalStream;
        }

        public string GetBufferedText()
        {
            _bufferStream.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_bufferStream, leaveOpen: true);
            return reader.ReadToEnd();
        }

        public long ContentLength => _bufferStream.Length;

        public override void Flush() => _originalStream.Flush();

        public override async Task FlushAsync(CancellationToken cancellationToken)
        {
            await _bufferStream.FlushAsync(cancellationToken);
            await _originalStream.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            _originalStream.Read(buffer, offset, count);

        public override void Write(byte[] buffer, int offset, int count)
        {
            _bufferStream.Write(buffer, offset, count);
            _originalStream.Write(buffer, offset, count);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await _bufferStream.WriteAsync(buffer, offset, count, cancellationToken);
            await _originalStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin) => 
            throw new NotSupportedException();

        public override void SetLength(long value) =>
            _originalStream.SetLength(value);

        public override bool CanRead => _originalStream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => _originalStream.CanWrite;

        public override long Length => _originalStream.Length;

        public override long Position
        {
            get => _originalStream.Position;
            set => throw new NotSupportedException();
        }
    }
}