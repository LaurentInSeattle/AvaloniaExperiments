namespace Lyt.Avalonia.Mvvm.Models;

public class ApplicationModelBase : IModel
{
    public void Initialize() => throw new NotImplementedException();
    public void Shutdown() => throw new NotImplementedException();
    public void SubscribeToUpdates(Action onUpdate, bool withUiDispatch = false) => throw new NotImplementedException();
}
