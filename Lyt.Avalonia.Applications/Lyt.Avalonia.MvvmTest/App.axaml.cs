namespace Lyt.Avalonia.MvvmTest;

public partial class App : ApplicationBase
{
    public App(): base(
        "MvvmTest",
        typeof(MainWindow),
        typeof(ApplicationModelBase), // Top level model 
        [
            // Models 
            typeof(TimingModel)
        ], 
        [
           // Singletons
           typeof(ShellViewModel)
        ], 
        [
            // Services 
            new Tuple<Type, Type>(typeof(ILogger), typeof(Logger)),
            new Tuple<Type, Type>(typeof(IMessenger), typeof(Messenger)),
        ])        
    {
    }

    // Why does it needs to be there ??? 
    public override void Initialize() => AvaloniaXamlLoader.Load(this);
}
