using Microsoft.Extensions.Hosting;

namespace Lyt.Avalonia.Mvvm;

public class ApplicationBase : Application
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    // The host cannot be null or else there is no app...
    public static IHost AppHost { get; private set; }
#pragma warning restore CS8618 

    private readonly string applicationKey;
    private readonly Type mainWindowType;
    private readonly Type applicationModelType;
    private readonly List<Type> modelTypes;
    private readonly List<Type> singletonTypes;
    private readonly List<Tuple<Type, Type>> servicesInterfaceAndType;
    private readonly List<Type> validatedModelTypes;

    public ApplicationBase(
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

    protected void InitializeHosting()
    {
        ApplicationBase.AppHost = Host.CreateDefaultBuilder()
                .ConfigureServices((_0, services) =>
                {
                    //// Always Main Window 
                    //_ = services.AddSingleton(typeof(Window), this.mainWindowType);

                    //// The Application Model, also  a singleton, no need here to also add it without the inferface  
                    //_ = services.AddSingleton(typeof(IApplicationModel), this.applicationModelType);

                    //// Models 
                    //foreach (Type modelType in this.modelTypes)
                    //{
                    //    bool isModel = typeof(IModel).IsAssignableFrom(modelType);
                    //    if (isModel)
                    //    {
                    //        // Models can be retrieved all via the interface or retrieved only one by providing specific type,
                    //        // just like singletons below
                    //        _ = services.AddSingleton(modelType);
                    //        this.validatedModelTypes.Add(modelType);
                    //    }
                    //    else
                    //    {
                    //        Debug.WriteLine(modelType.FullName!.ToString() + " is not a IModel");
                    //    }
                    //}

                    //// Singletons, they do not need an interface. 
                    //foreach (var singletonType in this.singletonTypes)
                    //{
                    //    _ = services.AddSingleton(singletonType);
                    //}

                    //// Services, all must comply to a specific interface 
                    //foreach (var serviceType in this.servicesInterfaceAndType)
                    //{
                    //    var interfaceType = serviceType.Item1;
                    //    var implementationType = serviceType.Item2;
                    //    _ = services.AddSingleton(interfaceType, implementationType);
                    //}

                    //_ = services.AddSingleton<ILogger, NoahLogger>();
                }).Build();
    }

    //public static T GetRequiredService<T>() where T : notnull
    //    => ApplicationBase.AppHost!.Services.GetRequiredService<T>();

    //public static T GetModel<T>() where T : notnull
    //{
    //    T? model = ApplicationBase.GetRequiredService<T>();
    //    if (model is null)
    //    {
    //        throw new ApplicationException("No model of type " + typeof(T).FullName);
    //    }

    //    bool isModel = typeof(IModel).IsAssignableFrom(typeof(T));
    //    if (!isModel)
    //    {
    //        throw new ApplicationException(typeof(T).FullName + "  is not a IMOdel");
    //    }

    //    return model;
    //}

    //public IEnumerable<IModel> GetModels()
    //{
    //    List<IModel> models = [];
    //    foreach (Type type in this.validatedModelTypes)
    //    {
    //        var model = ApplicationBase.AppHost!.Services.GetRequiredService(type);
    //        bool isModel = typeof(IModel).IsAssignableFrom(model.GetType());
    //        if (isModel)
    //        {
    //            models.Add((model as IModel)!);
    //        }
    //    }

    //    return models;
    //}
}