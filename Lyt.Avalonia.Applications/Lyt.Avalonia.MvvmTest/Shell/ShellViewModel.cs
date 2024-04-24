namespace Lyt.Avalonia.MvvmTest.Shell;

public sealed class ShellViewModel : Bindable<ShellView>
{
    public ShellViewModel() 
    {
    }

    public string Greeting => "Hello!";
}
