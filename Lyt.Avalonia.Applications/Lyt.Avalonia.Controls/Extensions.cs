using Avalonia.Reactive;

namespace Lyt.Avalonia.Controls;

public static class Extensions
{
    /// <summary> Gets a control </summary>
    /// <typeparam name="T">Type of the Control to get</typeparam>
    /// <param name="templatedControl">TemplatedControl owner of the Indicated Control</param>
    /// <param name="e">The TemplateAppliedEventArgs</param>
    /// <param name="name">The Name of the Control to return</param>
    /// <returns>a control with the indicated params</returns>
    public static T? GetControl<T>(this TemplatedControl templatedControl, TemplateAppliedEventArgs e, string name)
        where T : AvaloniaObject
        => e.NameScope.Find<T>(name);

    /// <summary>
    /// Gets a control
    /// </summary>
    /// <typeparam name="T">Type of the Control to get</typeparam>
    /// <param name="templatedControl">TemplatedControl owner of the Indicated Control</param>
    /// <param name="e">The TemplateAppliedEventArgs</param>
    /// <param name="name">The Name of the Control to return</param>
    /// <param name="avaloniaObj">a control with the indicated params</param>
    public static void GetControl<T>(
        this TemplatedControl templatedControl, TemplateAppliedEventArgs e, string name, out T? avaloniaObj) 
            where T : AvaloniaObject
                => avaloniaObj = GetControl<T>(templatedControl, e, name);

    // Summary:
    //     Subscribes an element handler to an observable sequence.
    // Parameters:
    //   source:
    //     Observable sequence to subscribe to.
    //   onNext:
    //     Action to invoke for each element in the observable sequence.
    // Type parameters:
    //   T:
    //     The type of the elements in the source sequence.
    // Returns:
    //     System.IDisposable object used to unsubscribe from the observable sequence.
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     source or onNext is null.
    public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (onNext == null)
        {
            throw new ArgumentNullException(nameof(onNext));
        }

        return source.Subscribe(new AnonymousObserver<T>(onNext, Stubs.Throw, Stubs.Nop));
    }

    // Summary:
    //     Subscribes an element handler and an exception handler to an observable sequence.
    // Parameters:
    //   source:
    //     Observable sequence to subscribe to.
    //   onNext:
    //     Action to invoke for each element in the observable sequence.
    //   onError:
    //     Action to invoke upon exceptional termination of the observable sequence.
    // Type parameters:
    //   T:
    //     The type of the elements in the source sequence.
    // Returns:
    //     System.IDisposable object used to unsubscribe from the observable sequence.
    // Exceptions:
    //   T:System.ArgumentNullException:
    //     source or onNext or onError is null.
    public static IDisposable Subscribe<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (onNext == null)
        {
            throw new ArgumentNullException(nameof(onNext));
        }

        if (onError == null)
        {
            throw new ArgumentNullException(nameof(onError));
        }

        return source.Subscribe(new AnonymousObserver<T>(onNext, onError, Stubs.Nop));
    }
}
