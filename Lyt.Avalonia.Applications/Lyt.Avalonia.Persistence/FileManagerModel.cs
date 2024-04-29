namespace Lyt.Avalonia.Persistence;

public sealed class FileManagerModel : ModelBase, IModel
{
    private static readonly string[] LogFilesFilters = ["*.log", "*.csv", "*.txt"];

    private const string LogsFolder = "Logs";

    // LATER 
    // private const string ConfigFolder = "Config";

    private FileManagerConfiguration configuration;

    public FileManagerModel() : base() 
        => this.configuration = new FileManagerConfiguration(string.Empty, string.Empty, string.Empty);

    public string ApplicationFolderPath { get; private set; } = string.Empty;

    public string ApplicationUserFolderPath { get; private set; } = string.Empty;

    public string ApplicationLogsFolderPath { get; private set; } = string.Empty;

    public override Task Initialize() => Task.CompletedTask;

    public override Task Shutdown() => Task.CompletedTask;

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
            throw new Exception("Invalid File Manager Configuration");
        }

        this.configuration = configuration;
        try
        {
            this.SetupEnvironment();
        }
        catch (Exception ex)
        {
            this.Logger.Error(ex.ToString());
            throw new Exception("File Manager failed to setup environment", ex);
        }

        if (string.IsNullOrWhiteSpace(this.ApplicationFolderPath) ||
            string.IsNullOrWhiteSpace(this.ApplicationLogsFolderPath) ||
            string.IsNullOrWhiteSpace(this.ApplicationUserFolderPath))
        {
            throw new Exception("File Manager failed to setup environment");
        }

        this.CleanupOldLogs();
        return Task.CompletedTask;
    }

    private void SetupEnvironment()
    {
        string directory =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                this.configuration.Organization);
        this.CreateFolderIfNeeded(directory);

        string subDirectory = Path.Combine(directory, this.configuration.Application);
        this.CreateFolderIfNeeded(subDirectory);
        this.ApplicationFolderPath = subDirectory;

        string logsFolder = Path.Combine(subDirectory, FileManagerModel.LogsFolder);
        this.CreateFolderIfNeeded(logsFolder);
        this.ApplicationLogsFolderPath = logsFolder;
        this.CleanupOldLogs();

        string userDirectory =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                this.configuration.Organization);
        this.CreateFolderIfNeeded(userDirectory);

        string userSubDirectory =
            Path.Combine(userDirectory, this.configuration.Application);
        this.CreateFolderIfNeeded(userSubDirectory);
        this.ApplicationUserFolderPath = userSubDirectory;
    }

    public static string TimestampString()
    {
        var now = DateTime.Now.ToLocalTime();
        return
            string.Format(
                "{0:D2}_{1:D2}_{2:D2}_{3:D2}_{4:D2}_{5:D2}",
                now.Year - 2000, now.Month, now.Day,
                now.Hour, now.Minute, now.Second);
    }

    public static string ShortTimestampString()
    {
        var now = DateTime.Now.ToLocalTime();
        return string.Format("{0:D1}_{1:D2}_{2:D2}", now.Year - 2020, now.Month, now.Day);
    }

    public void CreateFolderIfNeeded(string folderName)
    {
        string _ = FileManagerModel.ValidPathName(folderName, out bool changed);
        if (changed)
        {
            string msg = "Invalid folder name: " + folderName;
            this.Logger.Error(msg);
            throw new Exception(msg);
        }

        if (!Directory.Exists(folderName))
        {
            Directory.CreateDirectory(folderName);
        }
    }

    // Use only for path names, not including file names  
    public static string ValidPathName(string fileName, out bool changed)
    {
        string validFileName = fileName;
        foreach (char c in Path.GetInvalidPathChars())
        {
            validFileName = validFileName.Replace(c, '_');
        }

        // Spaces are valid in Windows path names but often cause problems, so avoid...
        validFileName = validFileName.Replace(' ', '_');
        changed = validFileName != fileName;
        return validFileName;
    }

    // Use for file names, not including folder path 
    public static string ValidFileName(string fileName, out bool changed)
    {
        string validFileName = fileName;
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            validFileName = validFileName.Replace(c, '_');
        }

        // Spaces are valid in Windows file names but often cause problems, so avoid...
        validFileName = validFileName.Replace(' ', '_');
        changed = validFileName != fileName;
        return validFileName;
    }

    private void CleanupOldLogs()
    {
        // Launch a fire & forget task to cleanup old logs 
        // If we have a debugger attached, we can delete old stuff somewhat more agressively 
        TimeSpan timeSpan = Debugger.IsAttached ? TimeSpan.FromDays(7) : TimeSpan.FromDays(31);
        Task.Run(async () =>
        {
            // Wait: Don't bog down startup 
            await Task.Delay(420);
            foreach(string filter in FileManagerModel.LogFilesFilters)
            {
                await this.CleanupOldFiles(timeSpan, this.ApplicationLogsFolderPath, filter);
            }
        });
    }

    public async Task CleanupOldFiles(TimeSpan since, string path, string filter)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        // Enumerates files 
        var enumerationOptions = new EnumerationOptions()
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = true,
            MatchType = MatchType.Simple,
            MaxRecursionDepth = 8,
        };
        var files = Directory.EnumerateFiles(path, filter, enumerationOptions);

        // Wait... 
        await Task.Delay(42);

        DateTime now = DateTime.Now;
        foreach (string file in files)
        {
            TimeSpan timeSpan = now - File.GetCreationTime(file);
            if (timeSpan > since)
            {
                try
                {
                    File.Delete(file);
                    this.Logger.Info(file + " deleted.");
                }
                catch (Exception ex)
                {
                    // Log and swallow 
                    this.Logger.Error(ex.ToString());
                }
                finally
                {
                    // Wait: Don't bog down startup 
                    await Task.Delay(42);
                }
            }
        }
    }
}
