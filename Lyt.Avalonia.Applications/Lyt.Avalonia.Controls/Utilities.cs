namespace Lyt.Avalonia.Controls;

public static class Utilities
{
    public static bool TryFindResource<T>(string resourceName, out T? resource)
    {
        resource = default;
        try
        {
            if (Application.Current is null )
            {
                return false;
            }

            bool found = Application.Current.TryFindResource(resourceName, out object? resourceObject);
            if (found && resourceObject is T resourceTypeT)
            {
                resource = resourceTypeT;
                return true;
            } 
        }
        catch (Exception ex)
        {
            if (Debugger.IsAttached) { Debugger.Break(); }
            Debug.WriteLine(ex);
        }

        return false;
    }
}
