namespace Lyt.Avalonia.Mvvm.Models;

public abstract class ModelBase : IModel
{
    protected readonly IMessenger messenger;

    public ModelBase(IMessenger messenger) => this.messenger = messenger;

    public abstract Task Initialize(); 

    public abstract Task Shutdown(); 

    public void SubscribeToUpdates(Action<ModelUpdateMessage> onUpdate, bool withUiDispatch = false)
        => this.messenger.Subscribe(onUpdate, withUiDispatch);

    protected void NotifyUpdate() => this.messenger.Publish(new ModelUpdateMessage(this));
}
