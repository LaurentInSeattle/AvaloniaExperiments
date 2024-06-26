﻿namespace Lyt.Avalonia.Interfaces;

public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
}

public interface ILogger
{
    void Debug(string message);
    void Info(string message);
    void Warning(string message);
    void Error(string message);
}
