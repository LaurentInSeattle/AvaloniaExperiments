namespace Lyt.Avalonia.Mvvm.Models;

public abstract class ModelBase : IModel
{
    public ModelBase()
    {
        this.Messenger = ApplicationBase.GetRequiredService<IMessenger>();
        this.Logger = ApplicationBase.GetRequiredService<ILogger>();
    }

    public abstract Task Initialize(); 

    public abstract Task Shutdown();

    public ILogger Logger { get; private set; }

    public IMessenger Messenger { get; private set; }

    public void SubscribeToUpdates(Action<ModelUpdateMessage> onUpdate, bool withUiDispatch = false)
        => this.Messenger.Subscribe(onUpdate, withUiDispatch);

    protected void NotifyUpdate() => this.Messenger.Publish(new ModelUpdateMessage(this));
}
