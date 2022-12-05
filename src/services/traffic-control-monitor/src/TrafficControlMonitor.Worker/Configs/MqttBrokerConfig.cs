#nullable disable
namespace TrafficControlMonitor.Worker.Configs;

public record MqttBrokerConfig
{
    public string Server { get; init; }
    
    public string[] Topics { get; init; }
    
    public MqttBrokerClientConfig ClientSettings { get; init; }
}

public record MqttBrokerClientConfig
{
    public string ClientId { get; init; }

    public string Username { get; init; }
    
    public string Password { get; init; }
}