
namespace Lyt.Avalonia.Mvvm.Core;

/// <summary> Strongly typed bindable </summary>
/// <typeparam name="TControl"></typeparam>
public class Bindable<TControl> : Bindable where TControl : Control, new()
{
    public Bindable() : base() { }

    public Bindable(TControl frameworkElement) : base() 
        => this.Bind(frameworkElement);

    public TControl? View => this.Control as TControl;
}
