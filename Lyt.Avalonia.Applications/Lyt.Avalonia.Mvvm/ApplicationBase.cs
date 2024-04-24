﻿namespace Lyt.Avalonia.Mvvm;

public class ApplicationBase : Application
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // The host cannot be null or else there is no app...
    public static IHost AppHost { get; private set; }

    // Logger will never be null or else the app did not take off
    public ILogger Logger { get; private set; }

    private readonly string applicationKey;
    private readonly Type mainWindowType;
    private readonly Type applicationModelType;
    private readonly List<Type> modelTypes;
    private readonly List<Type> singletonTypes;
    private readonly List<Tuple<Type, Type>> servicesInterfaceAndType;
    private readonly List<Type> validatedModelTypes;

    public ApplicationBase(
#pragma warning restore CS8618 
        string applicationKey,
        Type mainWindowType,
        Type applicationModelType,
        List<Type> modelTypes,
        List<Type> singletonTypes,
        List<Tuple<Type, Type>> servicesInterfaceAndType,
        bool isSingleInstance = false)
    {
        this.applicationKey = applicationKey;
        this.mainWindowType = mainWindowType;
        this.applicationModelType = applicationModelType;
        this.modelTypes = modelTypes;
        this.singletonTypes = singletonTypes;
        this.servicesInterfaceAndType = servicesInterfaceAndType;

        this.validatedModelTypes = [];

        //// Enforce single instance 
        //bool needToWait = false;
        //if (isSingleInstance)
        //{
        //    var executingAssembly = Assembly.GetExecutingAssembly();
        //    //while (AssemblyHelpers.IsAlreadyRunning(executingAssembly))
        //    //{
        //    //    AssemblyHelpers.KillOldestInstance(executingAssembly);
        //    //    needToWait = true;
        //    //}
        //}

        //if (needToWait)
        //{
        //    Schedule.OnUiThread(50, this.Initialize, DispatcherPriority.Normal);
        //}
        //else
        //{
        //    this.Initialize();
        //}
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        this.InitializeHosting();

        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var startupWindow = ApplicationBase.GetRequiredService<Window>();
            if (startupWindow is Window window)
            {
                desktop.MainWindow = window;
            }
            else
            {
                throw new NotImplementedException("Failed to create MainWindow");
            }
        }
        else if (this.ApplicationLifetime is ISingleViewApplicationLifetime)
        {
            // For designer mode
        }
        else
        {
            throw new NotImplementedException("Unsupported Application Lifetime");
        }

        base.OnFrameworkInitializationCompleted();

        await this.OnStartup(); 
    }

    private void InitializeHosting()
    {
        ApplicationBase.AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_0, services) =>
                {
                    // Register the app
                    _ = services.AddSingleton(typeof(ApplicationBase), this);

                    // Always Main Window 
                    _ = services.AddSingleton(typeof(Window), this.mainWindowType);

                    // The Application Model, also  a singleton, no need here to also add it without the inferface  
                    _ = services.AddSingleton(typeof(IApplicationModel), this.applicationModelType);

                    // Models 
                    foreach (Type modelType in this.modelTypes)
                    {
                        bool isModel = typeof(IModel).IsAssignableFrom(modelType);
                        if (isModel)
                        {
                            // Models can be retrieved all via the interface or retrieved only one by providing specific type,
                            // just like singletons below
                            _ = services.AddSingleton(modelType);
                            this.validatedModelTypes.Add(modelType);
                        }
                        else
                        {
                            Debug.WriteLine(modelType.FullName!.ToString() + " is not a IModel");
                        }
                    }

                    // Singletons, they do not need an interface. 
                    foreach (var singletonType in this.singletonTypes)
                    {
                        _ = services.AddSingleton(singletonType);
                    }

                    // Services, all must comply to a specific interface 
                    foreach (var serviceType in this.servicesInterfaceAndType)
                    {
                        var interfaceType = serviceType.Item1;
                        var implementationType = serviceType.Item2;
                        _ = services.AddSingleton(interfaceType, implementationType);
                    }

                    //_ = services.AddSingleton<ILogger, NoahLogger>();
                }).Build();
    }

    public static T GetRequiredService<T>() where T : notnull
        => ApplicationBase.AppHost!.Services.GetRequiredService<T>();

    public static T GetModel<T>() where T : notnull
    {
        T? model = ApplicationBase.GetRequiredService<T>() ?? throw new ApplicationException("No model of type " + typeof(T).FullName);
        bool isModel = typeof(IModel).IsAssignableFrom(typeof(T));
        if (!isModel)
        {
            throw new ApplicationException(typeof(T).FullName + "  is not a IMOdel");
        }

        return model;
    }

    public IEnumerable<IModel> GetModels()
    {
        List<IModel> models = [];
        foreach (Type type in this.validatedModelTypes)
        {
            object model = ApplicationBase.AppHost!.Services.GetRequiredService(type);
            bool isModel = typeof(IModel).IsAssignableFrom(model.GetType());
            if (isModel)
            {
                models.Add((model as IModel)!);
            }
        }

        return models;
    }
    
    private async Task OnStartup ()
    {
        await ApplicationBase.AppHost.StartAsync();

        var logger = ApplicationBase.GetRequiredService<ILogger>();
        this.Logger = logger;

        //if (logger is .... )
        //{
        //    noahLogger.ApplicationName = this.applicationKey;
        //    if (Debugger.IsAttached)
        //    {
        //        try
        //        {
        //            this.logViewer = new LogViewer();
        //            this.logViewer.Show();
        //            noahLogger.AttachCustomLogger(this.logViewer!);
        //        }
        //        catch (Exception) { /* swallow */ }
        //    }
        //}

        this.Logger.Info("***   Startup   ***");

        // Warming up the models: 
        // This ensures that the Application Model and all listed models are constructed.
        this.WarmupModels();
        IApplicationModel applicationModel = ApplicationBase.GetRequiredService<IApplicationModel>();
        await applicationModel.Initialize();
    }

    private void WarmupModels()
    {
        foreach (Type type in this.validatedModelTypes)
        {
            object model = ApplicationBase.AppHost!.Services.GetRequiredService(type);
            if ( model is not IModel)
            {
                throw new ApplicationException("Failed to warmup model: " + type.FullName );
            }
        }
    }

    public async Task OnShutdown()
    {
        this.Logger.Info("***   Shutdown   ***");
        //startupWindow.Closing += (_, _) => { this.logViewer?.Close(); };
        IApplicationModel applicationModel = ApplicationBase.GetRequiredService<IApplicationModel>();
        await applicationModel.Shutdown();
        await ApplicationBase.AppHost!.StopAsync();
    }
}