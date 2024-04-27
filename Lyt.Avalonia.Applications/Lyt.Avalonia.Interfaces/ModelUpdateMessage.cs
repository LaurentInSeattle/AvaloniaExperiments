namespace Lyt.Avalonia.Interfaces.Model;

public sealed class ModelUpdateMessage(IModel model, string? propertyName = "")
{
    public IModel Model { get; private set; } = model;

    public string? PropertyName { get; private set; } = propertyName;
}
