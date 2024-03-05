using Azure.Messaging.ServiceBus;

namespace LogsGenerator_AZ_Storage_Queue.Services;

public class ServiceBusService : IServiceBusService
{
    private readonly IFilesManager _filesManager;
    private readonly string _queueName;
    private readonly string _connectionString;
    
    public ServiceBusService(IFilesManager filesManager, IConfiguration configuration)
    {
        _filesManager = filesManager;
        _queueName = configuration["ServiceBus:QueueName"]!;
        var serviceBusSecretName = configuration["KeyVault:ServiceBusSecretName"];
        _connectionString = configuration[serviceBusSecretName!]!;
    }
    
    public async Task<(bool, int)> AddMessages(CancellationToken cancellationToken)
    {
        var addedMessages = 0;
        await using var client = new ServiceBusClient(_connectionString);
        await using var sender = client.CreateSender(_queueName);

        var filePaths = _filesManager.GetListLogFiles();
        if (!filePaths.Any())
        {
            return (false, addedMessages);
        }
        var messages = new List<ServiceBusMessage>();
        foreach (var filePath in filePaths)
        {
            var fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
            var fileContentAsBase64 = Convert.ToBase64String(fileBytes);
            
            var fileName = Path.GetFileName(filePath);
            
            var message = new ServiceBusMessage(fileContentAsBase64)
            {
                ApplicationProperties = { { "FileName", fileName } }
            };
            
            messages.Add(message);
            _filesManager.DeleteFile(filePath);
        }

        addedMessages = messages.Count;
        await sender.SendMessagesAsync(messages, cancellationToken);

        return (true, addedMessages);
    }

}