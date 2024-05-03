
namespace Lyt.Avalonia.Controls.Logging;

public partial class LogViewerWindow : Window, ILogger, INotifyPropertyChanged
{
    public new event PropertyChangedEventHandler? PropertyChanged;

    private readonly SolidColorBrush greenBrushDebug;
    private readonly SolidColorBrush greenBrushInfo;
    private readonly SolidColorBrush orangeBrush;
    private readonly SolidColorBrush redBrush;
    private ObservableCollection<LogEntry> observableLogEntries; 

    private List<LogEntry> AllLogEntries { get; set; }
    private bool showingAll;
    
    public LogViewerWindow()
    {
        this.InitializeComponent();
        this.showingAll = true;
        this.AllLogEntries = [];
        this.observableLogEntries = [];
        this.DataContext = this.ObservableLogEntries;
        this.greenBrushDebug = new SolidColorBrush(Color.FromRgb(0x5E, 0xD9, 0xBF));
        this.greenBrushInfo = new SolidColorBrush(Color.FromRgb(0x40, 0xE9, 0xAE));
        this.orangeBrush = new SolidColorBrush(Colors.DarkOrange);
        this.redBrush= new SolidColorBrush(Colors.Orchid);
    }

    public ObservableCollection<LogEntry> ObservableLogEntries 
    {
        get => this.observableLogEntries;  
        set
        {
            this.observableLogEntries = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ObservableLogEntries)));
        }
    }

    public void Debug(string message) 
    {
        System.Diagnostics.Debug.WriteLine(message);
        this.Log(LogLevel.Debug, message);
    }

    public void Info(string message)
    {
        Trace.TraceInformation(message);
        this.Log(LogLevel.Info, message);
    }


    public void Warning(string message)
    {
        Trace.TraceWarning(message);
        this.Log(LogLevel.Warning, message);
    }


    public void Error(string message)
    {
        Trace.TraceError(message);
        this.Log(LogLevel.Error, message);
    }

    private void Log(LogLevel logLevel, string message)
    {
        SolidColorBrush brush = this.redBrush;
        if (logLevel == LogLevel.Debug)
        {
            brush = this.greenBrushDebug;
        }
        else if (logLevel == LogLevel.Info)
        {
            brush = this.greenBrushInfo;
        }
        else if (logLevel == LogLevel.Warning) 
        {
            brush = this.orangeBrush;
        }

        string time = DateTime.Now.ToLongTimeString();
        message = string.Concat(time, " - ", message);
        this.Update(new LogEntry { LogLevel = logLevel, Brush = brush, Message = message });
    }

    private void OnShowButtonClick(object sender, RoutedEventArgs e)
    {
        this.showingAll = !this.showingAll;
        this.ShowButton.Content = !this.showingAll ? "Show All Log" : "Warning and Errors Only";
        if (this.showingAll)
        {
            this.ObservableLogEntries = new ObservableCollection<LogEntry>(this.AllLogEntries);
        }
        else
        {
            var filtered =
                (from entry in this.AllLogEntries
                 where (entry.LogLevel == LogLevel.Warning) || (entry.LogLevel == LogLevel.Error)
                 select entry).ToList();
            this.ObservableLogEntries = new ObservableCollection<LogEntry>(filtered);
        }

        this.DataContext = this.ObservableLogEntries;
    }

    private void Update(LogEntry logEntry)
        => Dispatcher.UIThread.Post(
            () =>
            {
                this.AllLogEntries.Add(logEntry);
                if (this.showingAll)
                {
                    this.ObservableLogEntries.Add(logEntry);
                }
                else
                {
                    if ((logEntry.LogLevel == LogLevel.Error) || (logEntry.LogLevel == LogLevel.Warning))
                    {
                        this.ObservableLogEntries.Add(logEntry);
                    }
                }

                this.itemsControl.ItemsSource = this.ObservableLogEntries;
            },
            DispatcherPriority.Background);
}