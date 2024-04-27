namespace Lyt.Avalonia.MvvmTest;

public partial class App : ApplicationBase
{
    public App() : base(
        "Lyt",
        "Avalonia.MvvmTest",
        typeof(MainWindow),
        typeof(ApplicationModelBase), // Top level model 
        [
            // Models 
            typeof(TimingModel),
            typeof(UserAdministrationModel),
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
    }

    // Why does it needs to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
