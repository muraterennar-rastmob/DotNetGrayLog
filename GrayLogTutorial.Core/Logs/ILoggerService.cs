namespace GrayLogTutorial.Core.Logs;

public interface ILoggerService
{
    void Information(string message, params object[] args);
    void Error(Exception exception, string message, params object[] args);
    void Fatal(Exception exception, string message, params object[] args);
}