namespace LogsGenerator_AZ_Storage_Queue.Services;

public interface IServiceBusService
{
    Task<(bool, int)> AddMessages(CancellationToken cancellationToken);
}