namespace Lyt.Avalonia.MvvmTest;

public partial class App : ApplicationBase
{
    public App(): base(
        "MvvmTest",
        typeof(MainWindow),
        typeof(ApplicationModelBase), // Top level model 
        [], // Models 
        [], // Singletons 
        [
            // Services 
            new Tuple<Type, Type>(typeof(IMessenger), typeof(Messenger)),
        ])        
    {
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.InitializeHosting(); 

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

}
