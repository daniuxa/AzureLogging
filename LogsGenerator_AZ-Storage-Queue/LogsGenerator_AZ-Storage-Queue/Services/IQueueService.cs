namespace LogsGenerator_AZ_Storage_Queue.Services;

public interface IQueueService
{
    Task<bool> AddMessage(Log log);
}