using Lyt.Avalonia.MvvmTest.Views;

namespace Lyt.Avalonia.MvvmTest.ViewModels;

public partial class MainViewModel : Bindable<MainView>
{
    public string Greeting => "Hello!";
}
