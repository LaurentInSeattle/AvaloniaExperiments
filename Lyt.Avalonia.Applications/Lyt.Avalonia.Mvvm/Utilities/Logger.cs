namespace Lyt.Avalonia.Mvvm.Utilities;

public sealed class Logger : ILogger
{
    public void Debug(string message) => System.Diagnostics.Debug.WriteLine(message);

    public void Info(string message) => Trace.TraceInformation(message);

    public void Warning(string message) => Trace.TraceWarning(message);

    public void Error(string message) => Trace.TraceError(message);
}
