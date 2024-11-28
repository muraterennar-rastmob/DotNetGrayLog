using Serilog;

namespace GrayLogTutorial.Core.Logs.Serilog;

public class SerilogLoggerService:ILoggerService
{
    public void Information(string message, params object[] args)
    {
        Log.Information(message, args);
    }

    public void Error(Exception exception, string message, params object[] args)
    {
        Log.Error(exception, message, args);
    }

    public void Fatal(Exception exception, string message, params object[] args)
    {
        Log.Fatal(exception, message, args);
    }
}