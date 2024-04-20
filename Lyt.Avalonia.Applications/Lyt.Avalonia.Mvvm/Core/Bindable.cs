namespace Lyt.Avalonia.Mvvm.Core;

[AttributeUsage(AttributeTargets.Property)]
public class DoNotLogAttribute : Attribute { }

/// <summary> Bindable class, aka a View Model.  </summary>
/// <remarks> All bound properties are held in a dictionary.</remarks>
public class Bindable : NotifyPropertyChanged
{
    // public static ILogger? Logger { get; set; }

    /// <summary> The bounds properties.</summary>
    protected readonly Dictionary<string, object?> properties;

    public Bindable() => this.properties = [];

    /// <summary> The framework element, its Data Context is this instance. </summary>
    /// <remarks> Aka, the "View" </remarks>
    public Control? FrameworkElement { get; private set; }

    /// <summary> Allows to disable logging when properties are changing so that we do not flood the logs. </summary>
    /// <remarks> Use for quickly changing properties, mouse, sliders, etc.</remarks>
    public bool DisablePropertyChangedLogging { get; set; }

    /// <summary> Binds any object, when possible.</summary>
    public object? XamlView
    {
        get => this.FrameworkElement;
        set
        {
            if (value is Control frameworkElement)
            {
                this.Bind(frameworkElement);
            }
        }
    }

    /// <summary> Binds a framework element and setup callbacks. </summary>
    public void Bind(Control frameworkElement)
    {
        this.FrameworkElement = frameworkElement;
        this.FrameworkElement.DataContext = this;
        this.OnDataBinding();
        this.FrameworkElement.Loaded += (s, e) => this.OnViewLoaded();
    }

    /// <summary> Unbinds this bindable. </summary>
    public void Unbind()
    {
        if (this.FrameworkElement != null)
        {
            if (this.FrameworkElement.DataContext != null)
            {
                this.FrameworkElement.DataContext = null;
            }

            this.FrameworkElement.Loaded -= (s, e) => this.OnViewLoaded();
        }
    }

    /// <summary> Unbinds the provided framework element. </summary>
    public static void Unbind(Control frameworkElement)
    {
        if (frameworkElement is not null)
        {
            if (frameworkElement.DataContext is Bindable bindable)
            {
                bindable.FrameworkElement = null; 
                frameworkElement.DataContext = null;
                frameworkElement.Loaded -= (s, e) => bindable.OnViewLoaded();
            }
        }
    }

    /// <summary> Invoked when this bindable is bound </summary>
    protected virtual void OnDataBinding() { }

    /// <summary> Invoked when this bindable Framework element is loaded. </summary>
    protected virtual void OnViewLoaded() { } 

    /// <summary> Gets the value of a property </summary>
    protected T? Get<T>([CallerMemberName] string? name = null)
    {
        if (name is null)
        {
            // Bindable.Logger?.Fatal("Get property: no name");
            throw new Exception("Get property: no name"); 
        }

        return this.properties.TryGetValue(name, out object? value) ? value == null ? default : (T)value : default;
    }

    /// <summary> Sets the value of a property </summary>
    /// <returns> True, if the value was changed, false otherwise. </returns>
    protected bool Set<T>(T? value, [CallerMemberName] string? name = null)
    {
        if (name is null)
        {
            // Bindable.Logger?.Fatal("Set property: no name");
            throw new Exception("Set property: no name");
        }

        if (Equals(value, this.Get<T>(name)))
        {
            return false;
        }

        this.properties[name] = value;
        this.OnPropertyChanged(name);
        if ( ! this.DisablePropertyChangedLogging)
        {
            Bindable.LogPropertyChanged(name, value);
        }

        return true;
    }

    /// <summary> Clear (and Dispose when applicable) all properties </summary>
    protected void Clear()
    {
        foreach (var property in this.properties.Values)
        {
            if (property is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        this.properties.Clear();
    }
    
    #region Debug Utilities 

    /// <summary> Logs that a property is changing. </summary>
    [Conditional("DEBUG")]    
    private static void LogPropertyChanged(string name, object? value)
    {
        int frameIndex = 1;
        string typeName;
        do
        {
            ++frameIndex;
            var frame = new StackFrame(frameIndex);
            var frameMethod = frame.GetMethod();
            if (frameMethod == null)
            {
                return;
            }

            typeName = frameMethod.DeclaringType!.Name;
        }
        while (typeName.StartsWith("Bindable"));

        ++frameIndex;
        var frameAbove = new StackFrame(frameIndex);
        var methodAbove = frameAbove.GetMethod();
        if (methodAbove is not null)
        {
            var logAttribute = methodAbove.GetCustomAttribute<DoNotLogAttribute>();
            if (logAttribute is not null)
            {
                return;
            }
        }

        string methodAboveName = methodAbove != null ? methodAbove.Name : "<none>";
        string message =
            string.Format(
                "From {0} in {1}: Property {2} changed to:   {3}",
                typeName, methodAboveName, name, value==null? "null": value.ToString());
        // Bindable.Logger?.Info(message);
    }

    //[Conditional("DEBUG")]
    //private static void Log()
    //{
    //    // TODO : Serialize all properties to JSon and then log the Json string 

    //}

    #endregion Debug Utilities 
}
