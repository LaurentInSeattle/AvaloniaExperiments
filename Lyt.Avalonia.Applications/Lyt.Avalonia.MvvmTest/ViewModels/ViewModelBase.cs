using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;

namespace Lyt.Avalonia.MvvmTest.ViewModels;

public class ViewModelBase : ObservableObject
{
    public ViewModelBase()
    {
        
    }

    public ICommand DoIt; 
}
