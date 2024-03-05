namespace LogsGenerator_AZ_Storage_Queue.Services;

public class FilesManager : IFilesManager
{
    private const string FolderPath = "Logs";
    public IEnumerable<string> GetListLogFiles()
    {
        var logFiles = Directory.EnumerateFiles(FolderPath, "log*")
            .Where(file => !IsFileInUse(file));

        return logFiles;
    }
    
    public bool DeleteFile(string path)
    {
        if (!File.Exists(path))
        {
            return false;
        }
        
        File.Delete(path);
        return true;

    }
    
    public bool IsFileInUse(string filePath)
    {
        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
    }
}