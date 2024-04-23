using Lyt.Avalonia.Interfaces.Model;

namespace Lyt.Avalonia.MvvmTest.Models; 

public sealed class TimingModel : ModelBase, IModel
{
    private DispatcherTimer? dispatcherTimer;

    public TimingModel(IMessenger messenger) : base(messenger) => this.TickCount = 10_000;

    public int TickCount { get; private set; }

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
        }
    }

    public void Stop()
    {
        if (this.dispatcherTimer is not null)
        {
            this.dispatcherTimer.Stop();
            this.dispatcherTimer.Tick -= this.OnDispatcherTimerTick;
            this.dispatcherTimer = null;
        }
    }

    private void OnDispatcherTimerTick(object? sender, EventArgs e) => ++this.TickCount;
}
