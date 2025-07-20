using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker;

public class MessageBrokerSettings
{
    [Required]
    public required QueueSettings Queue { get; set; }

    [Required]
    public required ConnectionSettings Connection { get; set; }
}

public class QueueSettings
{
    [Required]
    public required string Name { get; set; }
}

public class ConnectionSettings
{
    [Required]
    public required string HostName { get; set; }

    [Required]
    public required string UserName { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required int Port { get; set; }

    public string VirtualHost { get; set; } = "/";

    public ushort ConsumerDispatchConcurrency { get; set; } = 1;
    
    public ushort PrefetchCount { get; set; } = 1;
}
