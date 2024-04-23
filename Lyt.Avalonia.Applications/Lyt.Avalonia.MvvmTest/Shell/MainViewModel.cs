namespace Lyt.Avalonia.MvvmTest.Shell;

public partial class MainViewModel : Bindable<MainView>
{
    public MainViewModel()
    {
        
    }

    public string Greeting => "Hello!";

}
