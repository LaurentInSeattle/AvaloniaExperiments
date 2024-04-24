namespace Lyt.Avalonia.Interfaces.Model;

public sealed class ModelUpdateMessage(IModel model)
{
    public IModel Model { get; private set; } = model;
}
