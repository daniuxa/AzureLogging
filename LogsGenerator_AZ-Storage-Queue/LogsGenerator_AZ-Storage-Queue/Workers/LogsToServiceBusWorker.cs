using LogsGenerator_AZ_Storage_Queue.Services;

namespace LogsGenerator_AZ_Storage_Queue.Workers;

public class LogsToServiceBusWorker(IServiceBusService serviceBusService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var (isAddedMessages, countAddedMessages) = await serviceBusService.AddMessages(stoppingToken);
            var msg = isAddedMessages
                ? $"Successfully added {countAddedMessages} messages to service bus"
                : "Messages haven't been added to service bus";
            Console.WriteLine(msg);
            
            await Task.Delay(30000, stoppingToken);
        }
    }
}