namespace LogsGenerator_AZ_Storage_Queue.Services;

public interface IFilesManager
{
    IEnumerable<string> GetListLogFiles();
    bool DeleteFile(string path);
    bool IsFileInUse(string filePath);
}