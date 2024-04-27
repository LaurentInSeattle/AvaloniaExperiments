
namespace Lyt.Avalonia.Mvvm.Utilities;

public sealed class Profiler
{
    private bool isTimingStarted;
    private Stopwatch? stopwatch;

    public static async Task FullGcCollect(int delay = 0)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GCNotificationStatus status = GC.WaitForFullGCComplete();
        if (Debugger.IsAttached && status != GCNotificationStatus.NotApplicable)
        {
            Debug.WriteLine("GC Status: " + status.ToString());
        }

        if (delay > 0)
        {
            await Task.Delay(delay);
        }
    }

    public static int[] CollectionCounts()
        => [GC.CollectionCount(0), GC.CollectionCount(1), GC.CollectionCount(2)];

    [Conditional("DEBUG")]
    public void Track(
        string message,
        [CallerFilePath] string sourceFilePath = "",
        [CallerLineNumber] int sourceLineNumber = 0)
    {
        var frame = new StackFrame(1);
        var method = frame.GetMethod();
        if (method is null)
        {
            return;
        }

        var typeName = method.DeclaringType!.Name;
        var memberName = method.Name;
        Debug.WriteLine(
            string.Format(
                "***** {0} -- From {1}.{2} in {3}, line {4}",
                message, typeName, memberName, sourceFilePath, sourceLineNumber));
    }

    [Conditional("DEBUG")]
    public void StartTiming()
    {
        if (this.isTimingStarted)
        {
            Debug.WriteLine("Timing already started");
            return;
        }

        this.isTimingStarted = true;
        this.stopwatch = Stopwatch.StartNew();
    }

    [Conditional("DEBUG")]
    public void EndTiming(string comment)
    {
        if (!this.isTimingStarted || (this.stopwatch == null))
        {
            Debug.WriteLine("Timing not started");
            return;
        }

        this.isTimingStarted = false;
        this.stopwatch.Stop();
        float millisecs = (float)Math.Round(this.stopwatch.Elapsed.TotalMilliseconds, 1);
        this.stopwatch = null;
        string rightNow = DateTime.Now.ToLocalTime().ToLongTimeString();
        Debug.WriteLine("***** " + comment + " - Timing: " + millisecs.ToString("F1") + " ms.  - at: " + rightNow);
    }

    [Conditional("DEBUG")]
    public static void MemorySnapshot(string comment = "")
    {
        if (OperatingSystem.IsWindows())
        {
            Profiler.WindowsMemorySnapshot(comment);
        }
    }

    [Conditional("DEBUG")]
    [SupportedOSPlatform("windows")]
    public static async void WindowsMemorySnapshot(string comment)
    {
        string rightNow = DateTime.Now.ToLocalTime().ToLongTimeString();
        Debug.WriteLine("***** Memory Snapshot " + comment + "  at: " + rightNow);
        await Profiler.FullGcCollect(50);
        var currentProcess = Process.GetCurrentProcess();
        string processName = currentProcess.ProcessName;
        var ctr1 = new PerformanceCounter("Process", "Private Bytes", processName);
        float privateBytes = ctr1.NextValue();
        int megaPrivateBytes = (int)((privateBytes + 512 * 1024) / (1024 * 1024));
        Debug.WriteLine("***** Private Bytes: " + megaPrivateBytes.ToString() + " MB.");
        ctr1.Dispose();

        // In dotNet 6, looking up those Perf Counter creates: 
        // Exception thrown: 'System.InvalidOperationException' in System.Diagnostics.PerformanceCounter.dll
        // Was working nicely in 4.xxx
        // 
        // var ctr2 = new PerformanceCounter(".NET CLR Memory", "# Gen 0 Collections", processName);
        // And a bunch more of them... 
    }
}
