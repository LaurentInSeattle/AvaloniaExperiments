namespace Lyt.Avalonia.MvvmTest.Shell;

public partial class MainWindow : Window
{
    private bool isShutdownRequested;

    private bool isShutdownComplete;

    public MainWindow()
    {
        this.InitializeComponent();

        this.Closing += this.OnMainWindowClosing;
        this.Loaded +=
            (s, e) =>
                this.Content =
                    Binder<ShellView, ShellViewModel>.CreateAndBind(App.GetRequiredService<ShellViewModel>()).View;
    }

    private void OnMainWindowClosing(object? sender, CancelEventArgs e)
    {
        if (!this.isShutdownComplete)
        {
            e.Cancel = true;
        }

        if (!this.isShutdownRequested)
        {
            this.isShutdownRequested = true;
            Schedule.OnUiThread(50,
                async () =>
                {
                    var app = App.GetRequiredService<ApplicationBase>();
                    await app.OnShutdown();
                    this.isShutdownComplete = true;
                    this.Close();
                }, DispatcherPriority.Normal);
        }
    }
}
