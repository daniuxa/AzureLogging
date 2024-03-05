using LogsGenerator_AZ_Storage_Queue.Services;
using Serilog.Events;

namespace LogsGenerator_AZ_Storage_Queue.Workers;

public class LogsToQueueWorker(IConfiguration configuration, IQueueService queueService, IServiceBusService serviceBusService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var serviceName = configuration["ServiceName"];
        if (serviceName == null)
        {
            return;
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var log = await LoggingTask(serviceName, stoppingToken);
            var addedToStorageQueue = await queueService.AddMessage(log);

            if (!addedToStorageQueue)
            {
                Console.WriteLine("Error: message hasn't been added to the queue");
                continue;
            }
            
            if (log.Message != null)
            {
                Serilog.Log.Write((LogEventLevel)log.LogLevel, log.Message);
            }

            Console.WriteLine("Successfully added message to the queue");
        }
    }

    private static async Task<Log> LoggingTask(string serviceName, CancellationToken cancellationToken)
    {
        var rand = new Random();
        var logLevel = (LogLevel)rand.Next(1, 4);
        var timeDelay = 4000;
        var log = new Log()
        {
            LogLevel = logLevel,
            ServiceName = serviceName
        };
        switch (logLevel)
        {
            case LogLevel.Debug:
                log.Message = $"Debug log at {DateTimeOffset.Now}";
                timeDelay = 4000;
                break;
            case LogLevel.Info:
                log.Message = $"Info log at {DateTimeOffset.Now}";
                timeDelay = 6000;
                break;
            case LogLevel.Warning:
                log.Message = $"Warning log at {DateTimeOffset.Now}";
                timeDelay = 8000;
                break;
            case LogLevel.Error:
                log.Message = $"Error log at {DateTimeOffset.Now}";
                timeDelay = 10000;
                break;
        }

        await Task.Delay(timeDelay, cancellationToken);
        return log;
    }
}