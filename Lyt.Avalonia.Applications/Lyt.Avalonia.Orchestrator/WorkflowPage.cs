﻿namespace Lyt.Avalonia.Orchestrator;

public class WorkflowPage<TState, TTrigger> : Bindable
    where TState : struct, Enum
    where TTrigger : struct, Enum
{
    public WorkflowPage()
    {
            
    }

    public virtual TState State => default;

    public virtual string Title => "Unknown";

    public virtual Task OnInitialize() => Task.CompletedTask;

    public virtual Task OnShutdown() => this.OnDeactivateAsync(default);

    public virtual Task OnActivateAsync(TState fromState) => Task.CompletedTask;

    public virtual Task OnDeactivateAsync(TState toState) => Task.CompletedTask;

    public virtual Task OnAction() => Task.CompletedTask;
}
