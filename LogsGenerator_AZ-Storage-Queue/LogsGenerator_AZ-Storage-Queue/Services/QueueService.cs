using System.Text.Json;
using Azure.Storage.Queues;

namespace LogsGenerator_AZ_Storage_Queue.Services;

public class QueueService(IConfiguration configuration) : IQueueService
{

    public async Task<bool> AddMessage(Log log)
    {
        try
        {
            var storageSecretName = configuration["KeyVault:StorageSecretName"];
            var connectionString = configuration[storageSecretName!];
            var queueName = configuration["AzureStorage:QueueName"];
            
            var queueClient = new QueueClient
                (connectionString, queueName);
            
            await queueClient.CreateIfNotExistsAsync();
            
            if (await queueClient.ExistsAsync())
            {
                var jsonLog = JsonSerializer.Serialize(log);
                await queueClient.SendMessageAsync(jsonLog);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }          
    }
}