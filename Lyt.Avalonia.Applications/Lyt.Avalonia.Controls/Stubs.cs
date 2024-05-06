namespace Lyt.Avalonia.Controls;

internal static class Stubs
{
    public static readonly Action Nop = delegate { };

    public static readonly Action<Exception> Throw = delegate (Exception ex) { throw(ex); };
}
