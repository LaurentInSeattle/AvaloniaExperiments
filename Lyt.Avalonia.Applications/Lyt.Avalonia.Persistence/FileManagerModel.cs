namespace Lyt.Avalonia.Persistence;

public sealed class FileManagerModel : ModelBase, IModel
{
    private FileManagerConfiguration configuration;

    public FileManagerModel()
    {
        this.configuration = new FileManagerConfiguration(string.Empty, string.Empty, string.Empty);
    }

    public override Task Initialize() { return Task.CompletedTask; }

    public override Task Shutdown() { return Task.CompletedTask; }

    public override Task Configure(object? modelConfiguration)
    {
        if (modelConfiguration is not FileManagerConfiguration configuration)
        {
            throw new ArgumentNullException(nameof(modelConfiguration));
        }

        if (string.IsNullOrWhiteSpace(configuration.Organization) ||
            string.IsNullOrWhiteSpace(configuration.Application) || 
            string.IsNullOrWhiteSpace(configuration.RootNamespace))
        {
            throw new Exception("Invalid File Manager Configuration" );
        }

        this.configuration = configuration;
        return Task.CompletedTask;
    }
}
