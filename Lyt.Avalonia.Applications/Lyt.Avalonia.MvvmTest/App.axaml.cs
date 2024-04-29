namespace Lyt.Avalonia.MvvmTest;

public partial class App : ApplicationBase
{
    public const string Organization = "Lyt";
    public const string Application = "AvaloniaMvvmTest";
    public const string RootNamespace = "Lyt.Avalonia.MvvmTest";

    public App() : base(
        App.Organization,
        App.Application,
        App.RootNamespace,
        typeof(MainWindow),
        typeof(ApplicationModelBase), // Top level model 
        [
            // Models 
            typeof(TimingModel),
            typeof(UserAdministrationModel),
            typeof(FileManagerModel),
        ],
        [
           // Singletons
           typeof(ShellViewModel),
           typeof(StartupViewModel),
           typeof(LoginViewModel),
           typeof(SelectViewModel),
           typeof(ProcessViewModel),
        ],
        [
            // Services 
            new Tuple<Type, Type>(typeof(ILogger), typeof(Logger)),
            new Tuple<Type, Type>(typeof(IMessenger), typeof(Messenger)),
        ],
        singleInstanceRequested: true)
    {
        // This should be empty, use the OnStartup override
    }

    protected override async Task OnStartup()
    {
        var fileManager = App.GetRequiredService<FileManagerModel>();
        await fileManager.Configure(new FileManagerConfiguration(App.Organization, App.Application, App.RootNamespace));
    }

    // Why does it needs to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
