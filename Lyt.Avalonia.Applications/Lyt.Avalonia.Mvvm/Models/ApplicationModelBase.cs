namespace Lyt.Avalonia.Mvvm.Models;

public class ApplicationModelBase : IApplicationModel
{
    private readonly ILogger logger;
    private readonly ApplicationBase application; 

    public ApplicationModelBase(ILogger logger, ApplicationBase application)
    {
        this.logger = logger;
        this.application = application;
    }

    public void Initialize()
    {
        try
        {
            foreach (var model in this.application.GetModels())
            {
                model.Initialize();
            }
        }
        catch (Exception ex)
        {
            // Should never fail here
            if (Debugger.IsAttached) { Debugger.Break(); }
            this.logger.Error(ex.ToString());
            throw new ApplicationException("Failed to initialize models.", ex);
        }

        try
        {
            // Launch cleanup threads 
            // FileHelpers.LaunchAllCleanupThreads();
            //await MiniProfiler.FullGcCollect();
            //MiniProfiler.MemorySnapshot("System software initialization complete");
        }
        catch (Exception ex)
        {
            // Should never fail here
            if (Debugger.IsAttached) { Debugger.Break(); }
            this.logger.Error(ex.ToString());
            throw new ApplicationException("Failed to cleanup on startup.", ex);
        }
    }

    public void Shutdown()
    {
        try
        {
            foreach (var model in this.application.GetModels())
            {
                model.Shutdown();
            }
        }
        catch (Exception ex)
        {
            // Should never fail here
            if (Debugger.IsAttached) { Debugger.Break(); }
            this.logger.Error(ex.ToString());
            throw;
        }
    }
}
