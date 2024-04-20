
namespace Lyt.Avalonia.Mvvm.Core;

/// <summary> Strongly typed bindable </summary>
/// <typeparam name="TFrameworkElement"></typeparam>
public class Bindable<TFrameworkElement> : Bindable where TFrameworkElement : Control, new()
{
    public Bindable() : base() { }

    public Bindable(TFrameworkElement frameworkElement) : base() => this.Bind(frameworkElement);

    public TFrameworkElement? View => this.FrameworkElement as TFrameworkElement;
}
