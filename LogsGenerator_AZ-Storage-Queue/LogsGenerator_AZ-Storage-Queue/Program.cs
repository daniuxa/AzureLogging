using Azure.Identity;
using LogsGenerator_AZ_Storage_Queue.Services;
using LogsGenerator_AZ_Storage_Queue.Workers;
using Serilog;
using Log = Serilog.Log;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs\\log.txt", rollingInterval: RollingInterval.Minute)
    .CreateLogger();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        
        var keyVaultName = config.Build()["KeyVault:KeyVaultName"];
        try
        {
            config.AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential());
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            throw;
        }
    })
    .ConfigureServices((_, services) =>
    {
        services.AddHostedService<LogsToQueueWorker>();
        services.AddHostedService<LogsToServiceBusWorker>();
        services.AddSingleton<IQueueService, QueueService>();
        services.AddSingleton<IServiceBusService, ServiceBusService>();
        services.AddSingleton<IFilesManager, FilesManager>();
    });

var host = builder.Build();
host.Run();