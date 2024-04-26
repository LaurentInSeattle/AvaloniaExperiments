namespace Lyt.Avalonia.Orchestrator;

public class WorkflowPage<TState, TTrigger> : Bindable
    where TState : struct, Enum
    where TTrigger : struct, Enum
{
    public WorkflowPage(WorkflowManager<TState, TTrigger> workflowManager)
    {
        this.WorkflowManager = workflowManager;
    }

    public virtual TState State => default;

    public virtual string Title => "Unknown";

    public virtual Task OnInitialize() => Task.CompletedTask;

    public virtual Task OnShutdown() => this.OnDeactivateAsync(default);

    public virtual Task OnActivateAsync(TState fromState) => Task.CompletedTask;

    public virtual Task OnDeactivateAsync(TState toState) => Task.CompletedTask;

    public WorkflowManager<TState, TTrigger> WorkflowManager { get; private set; }
}
