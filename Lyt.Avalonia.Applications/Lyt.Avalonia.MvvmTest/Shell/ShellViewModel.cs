namespace Lyt.Avalonia.MvvmTest.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    private readonly TimingModel timingModel;

    public ShellViewModel()
    {
        this.timingModel = ApplicationBase.GetModel<TimingModel>();
        this.timingModel.SubscribeToUpdates(this.OnTimingModelUpdated, withUiDispatch: true);
        this.TickCount = "Hello Avalonia!";
        this.IsTicking = string.Empty;
        this.OnStartStopCommand = new Command(this.OnStartStop);
    }

    public WorkflowManager<WorkflowState, WorkflowTrigger>? Workflow { get; private set; }

    private void OnStartStop(object? _)
    {
        if (this.timingModel.IsTicking)
        {
            this.timingModel.Stop();
        }
        else
        {
            this.timingModel.Start();
        }
    }

    protected override async void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.timingModel.Start();
        this.SetupWorkflow();
        if (this.Workflow is not null)
        {
            await this.Workflow.Initialize();
            _ = this.Workflow.Start();
        }
    }

    private void OnTimingModelUpdated(ModelUpdateMessage _)
    {
        int ticks = this.timingModel.TickCount;
        this.TickCount = string.Format("Ticks: {0}", ticks);
        bool modelIsTicking = this.timingModel.IsTicking;
        this.IsTicking = modelIsTicking ? "Ticking" : "Stopped";
        this.ButtonText = modelIsTicking ? "Stop" : "Start";
        Profiler.MemorySnapshot();
        if (this.Workflow is not null)
        {
            var fireAndForget = this.Workflow.Next();
        }
    }

    private void SetupWorkflow()
    {
        StateDefinition<WorkflowState, WorkflowTrigger, Bindable> Create<TViewModel, TView>(
            WorkflowState state, WorkflowTrigger trigger, WorkflowState target)
            where TViewModel : Bindable, new()
            where TView : Control, new()
        {
            var vm = App.GetRequiredService<TViewModel>();
            vm.Bind(new TView());
            if (vm is WorkflowPage<WorkflowState, WorkflowTrigger> page)
            {
                page.State = state;
                page.Title = state.ToString();
                return
                    new StateDefinition<WorkflowState, WorkflowTrigger, Bindable>(
                        state, page, null, null, null, null,
                        [
                            new TriggerDefinition<WorkflowState, WorkflowTrigger> ( trigger, target , null )
                        ]);
            }
            else
            {
                string msg = "View is not a Workflow Page";
                this.Logger.Error(msg);
                throw new Exception(msg);
            }
        }

        var startup = Create<StartupViewModel, StartupView>(WorkflowState.Startup, WorkflowTrigger.Ready, WorkflowState.Login);
        var login = Create<LoginViewModel, LoginView>(WorkflowState.Login, WorkflowTrigger.LoggedIn, WorkflowState.Select);
        var select = Create<SelectViewModel, SelectView>(WorkflowState.Select, WorkflowTrigger.Selected, WorkflowState.Process);
        var process = Create<ProcessViewModel, ProcessView>(WorkflowState.Process, WorkflowTrigger.Complete, WorkflowState.Login);

        var stateMachineDefinition =
            new StateMachineDefinition<WorkflowState, WorkflowTrigger, Bindable>(
                WorkflowState.Startup, // Initial state
                [ 
                    // List of states
                    startup, login , select, process,
                ]);

        this.Workflow =
            new WorkflowManager<WorkflowState, WorkflowTrigger>(
                this.Logger, this.Messenger, this.View!.WorkflowContent!, stateMachineDefinition);
    }

    public ICommand? OnStartStopCommand { get => this.Get<ICommand>(); set => this.Set(value); }

    public string? ButtonText { get => this.Get<string>(); set => this.Set(value); }

    public string? TickCount { get => this.Get<string>(); set => this.Set(value); }

    public string? IsTicking { get => this.Get<string>(); set => this.Set(value); }
}
