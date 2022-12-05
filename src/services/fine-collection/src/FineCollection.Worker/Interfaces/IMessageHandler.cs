namespace FineCollection.Worker.Interfaces;

public interface IMessageHandler
{
    Task ConsumeAsync(
        Func<object, BasicDeliverEventArgs, Task> consumerHandler,
        CancellationToken cancellationToken = default);
}