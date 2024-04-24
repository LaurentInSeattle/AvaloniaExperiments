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

    private void OnStartStop(object? _)
    {
        if ( this.timingModel.IsTicking)
        {
            this.timingModel.Stop();
        }
        else
        {
            this.timingModel.Start();
        }
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.timingModel.Start();
    }

    private void OnTimingModelUpdated(ModelUpdateMessage _)
    {
        int ticks = this.timingModel.TickCount;
        this.TickCount = string.Format("Ticks: {0}", ticks);
        bool modelIsTicking = this.timingModel.IsTicking;
        this.IsTicking = modelIsTicking ? "Ticking" : "Stopped";
        this.ButtonText = modelIsTicking ? "Stop" : "Start";
        Profiler.MemorySnapshot();
    }

    public ICommand? OnStartStopCommand { get => this.Get<ICommand>(); set => this.Set(value); }

    public string? ButtonText { get => this.Get<string>(); set => this.Set(value); }

    public string? TickCount { get => this.Get<string>(); set => this.Set(value); }

    public string? IsTicking { get => this.Get<string>(); set => this.Set(value); }
}
