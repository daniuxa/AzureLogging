namespace LogsGenerator_AZ_Storage_Queue;

public class Log
{
    public LogLevel LogLevel { get; set; }
    public required string ServiceName { get; set; }
    public string? Message { get; set; }
}


public enum LogLevel
{
    Debug = 1,
    Info = 2,
    Warning = 3,
    Error = 4
}