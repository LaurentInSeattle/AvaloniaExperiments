﻿namespace Lyt.Avalonia.Mvvm.Core;

public sealed class Command : ICommand
{
    // For a "Classic" Command based on delegates 
    private readonly Predicate<object?>? canExecute;
    private readonly Action<object?>? execute;

    // For an autogenerated command based on name convention for ICommand Source (Buttons) 
    private readonly MethodInfo? executeMethod;
    private readonly object? executeObject;

    public Command(Predicate<object?> canExecute, Action<object?> execute)
    {
        this.canExecute = canExecute;
        this.execute = execute;
    }

    public Command(Action<object?> execute)
    {
        this.canExecute = (o) => true;
        this.execute = execute;
    }

    public Command(MethodInfo methodInfo, object target)
    {
        if ((methodInfo == null) || (target == null))
        {
            return;
        }

        this.canExecute = (o) => true;
        this.executeMethod = methodInfo;
        this.executeObject = target;
    }

#pragma warning disable 0067 // Never used 
    public event EventHandler? CanExecuteChanged ; // For interface compliance 
#pragma warning restore 0067

    public bool CanExecute(object? parameter) => this.canExecute == null || this.canExecute(parameter);

    public void Execute(object? parameter)
    {
        if (this.execute != null)
        {
            Command.LogCommand(this.execute.Target, this.execute.Method.Name, parameter);
            this.execute.Invoke(parameter);
        }
        else if (this.executeMethod != null)
        {
            Command.LogCommand(this.executeObject, this.executeMethod.Name, parameter);
            this.executeMethod.Invoke(executeObject, new object?[] { parameter });
        }
    }

    [Conditional("DEBUG")]
    private static void LogCommand(object? target, string? name, object? parameter)
    {
        string targetName = target == null ? "null target" : target.GetType().Name;
        string? parameterString = parameter == null ? "null parameter" : parameter.ToString();
        string message = string.Format("Commanding {0}.{1} with parameter: {2}", targetName, name, parameterString);
        // Command.Logger?.Info(message);
    }
}
