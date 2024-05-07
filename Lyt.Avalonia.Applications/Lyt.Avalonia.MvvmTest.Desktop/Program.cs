using System;
using System.Diagnostics;
using Avalonia;

namespace Lyt.Avalonia.MvvmTest.Desktop;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        } 
        catch (Exception ex)
        {
            // Not good, but that's we have for now 
            if (Debugger.IsAttached) { Debugger.Break(); }
            Console.WriteLine(ex.ToString());
        }
    } 

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>().UsePlatformDetect().WithInterFont().LogToTrace();
}
