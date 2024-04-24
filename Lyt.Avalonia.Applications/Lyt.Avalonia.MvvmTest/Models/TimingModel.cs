namespace Lyt.Avalonia.MvvmTest.Models; 

public sealed class TimingModel() : ModelBase(), IModel
{
    private bool isTicking;
    private DispatcherTimer? dispatcherTimer;

    public int TickCount { get; private set; } = 10_000;

    public bool IsTicking 
    { 
        get => this.isTicking; 
        private set
        {
            if (this.isTicking != value)
            {
                this.isTicking = value;
                base.NotifyUpdate();
            }
        }
    }

    public override Task Initialize()
    {
        this.Start();
        return Task.CompletedTask;
    }

    public override Task Shutdown()
    {
        this.Stop();
        return Task.CompletedTask;
    }

    public void Start ()
    {
        if ( this.dispatcherTimer is null)
        {
            this.dispatcherTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5), IsEnabled = false };
            this.dispatcherTimer.Tick += this.OnDispatcherTimerTick;
            this.dispatcherTimer.Start();
            this.IsTicking = true;
        }
    }

    public void Stop()
    {
        if (this.dispatcherTimer is not null)
        {
            this.dispatcherTimer.Stop();
            this.dispatcherTimer.Tick -= this.OnDispatcherTimerTick;
            this.dispatcherTimer = null;
            this.IsTicking = false;
        }
    }

    private void OnDispatcherTimerTick(object? sender, EventArgs e)
    {
        ++this.TickCount;
        base.NotifyUpdate();
    } 
}
