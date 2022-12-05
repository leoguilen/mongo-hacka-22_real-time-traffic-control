using TrafficControlMonitor.Worker.Infra.Abstractions;
using TrafficControlMonitor.Worker.Services;

namespace TrafficControlMonitor.Worker;

public class Worker : IHostedService
{
    private readonly IEventBus _eventBus;
    private readonly ITrafficEventIngestionService _trafficEventIngestionService;
    private readonly IHostApplicationLifetime _appLifetime;

    public Worker(
        IEventBus eventBus,
        ITrafficEventIngestionService trafficEventIngestionService,
        IHostApplicationLifetime appLifetime)
    {
        _eventBus = eventBus;
        _trafficEventIngestionService = trafficEventIngestionService;
        _appLifetime = appLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
        => await _eventBus.StartConsumeAsync(_trafficEventIngestionService.ExecuteAsync, cancellationToken);

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _appLifetime.StopApplication();
        await _eventBus.StopConsumeAsync(cancellationToken);
    }
}
