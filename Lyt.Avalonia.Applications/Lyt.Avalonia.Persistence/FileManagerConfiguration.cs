namespace Lyt.Avalonia.Persistence;

public sealed class FileManagerConfiguration(string organization, string application, string rootNamespace)
{
    public string Organization { get; private set; } = organization;

    public string Application { get; private set; } = application;

    public string RootNamespace { get; private set; } = rootNamespace;
}
