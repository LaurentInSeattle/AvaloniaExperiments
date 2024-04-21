namespace Lyt.Avalonia.Interfaces.Model;

public interface IModel
{
    /// <summary>  Initializes the model. </summary>
    void Initialize();

    /// <summary> Shutdowns the model </summary>
    void Shutdown();

    /// <summary> Subscribes to updates from the model </summary>
    void SubscribeToUpdates(Action onUpdate, bool withUiDispatch = false);
}
