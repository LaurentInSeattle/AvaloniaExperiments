namespace Lyt.Avalonia.Interfaces.Model;

public sealed class ModelUpdateMessage
{
    public ModelUpdateMessage(IModel model) => this.Model = model;
    
    public IModel Model { get; private set; }   
}
