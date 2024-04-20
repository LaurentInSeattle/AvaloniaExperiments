namespace Lyt.Avalonia.Mvvm.Core;

/// <summary> Utility class to help binding views and view models with matching types. </summary>
/// <typeparam name="TControl"></typeparam>
/// <typeparam name="TBindable"></typeparam>
public static class Binder<TControl, TBindable>
    where TControl : Control, new()
    where TBindable : Bindable<TControl>
{
    public static TBindable CreateAndBind(TBindable? bindable = null)
    {
        if (bindable == null)
        {
            try
            {
                // Not an IoC Export: try a default CTOR using the activator 
                bindable = Activator.CreateInstance<TBindable>();
            }
            catch (Exception e)
            {
                //Bindable.Logger?.Fatal( "Failed to create an instance of " + typeof(TBindable).Name);
                //Bindable.Logger?.Fatal(e);
                throw;
            }
        }

        var control = new TControl();
        bindable.Bind(control);
        return bindable;
    }

    public static void Bind(TBindable bindable)
    {
        var control = new TControl();
        bindable.Bind(control);
    }

    public static TBindable Bind(TControl control)
    {
        TBindable bindable;
        try
        {
            // Not an IoC Export: try a default CTOR using the activator 
            bindable = Activator.CreateInstance<TBindable>();
        }
        catch (Exception e)
        {
            //Bindable.Logger?.Fatal("Failed to create an instance of " + typeof(TBindable).Name);
            //Bindable.Logger?.Fatal(e);
            throw;
        }

        bindable.Bind(control);
        return bindable;
    }

    public static void Bind(TControl control, TBindable bindable)
    {
        bindable.Bind(control);
    }
}
