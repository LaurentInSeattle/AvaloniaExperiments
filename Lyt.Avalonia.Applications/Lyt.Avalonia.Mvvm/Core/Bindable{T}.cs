
using Avalonia.Controls;

namespace Lyt.Avalonia.Mvvm.Core;

/// <summary> Strongly typed bindable </summary>
/// <typeparam name="TControl"></typeparam>
public class Bindable<TControl> : Bindable where TControl : Control, new()
{
    public Bindable() : base() { }

    public Bindable(TControl control) : base() 
        => this.Bind(control);

    public void CreateViewAndBind()
    {
        var view = new TControl();
        this.Bind(view);
    }

    public TControl? View => this.Control as TControl;
}
