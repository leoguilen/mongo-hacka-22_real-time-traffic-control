#nullable disable
namespace FineCollection.Worker.Configs;

public record AmqpBrokerConfig
{
    public string Server { get; init; }

    public string Username { get; init; }

    public string Password { get; init; }

    public string QueueName { get; init; }

    public string ClientId { get; init; }
}