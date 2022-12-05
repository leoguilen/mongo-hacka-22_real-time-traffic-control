namespace TrafficControlMonitor.Worker.Infra.EventBus.Mosquitto;

internal class MosquittoEventBus : IEventBus, IAsyncDisposable
{
    private static readonly IMqttClient s_mqttClient = new MqttFactory().CreateMqttClient();

    private readonly MqttBrokerConfig _brokerConfig;
    private readonly ILogger<MosquittoEventBus> _logger;

    private bool _disposed;
    
    public MosquittoEventBus(
        IOptions<MqttBrokerConfig> brokerConfig,
        ILogger<MosquittoEventBus> logger)
    {
        _brokerConfig = brokerConfig.Value;
        _logger = logger;
    }

    public async Task StartConsumeAsync(
        Func<string, TrafficEventType, CancellationToken, Task> messageReceivedHandler,
        CancellationToken cancellationToken)
    {
        s_mqttClient.ConnectedAsync += async (_) =>
        {
            _logger.LogDebug("Connected to MQTT broker with client ID {ClientId}",
                _brokerConfig.ClientSettings.ClientId);
            await InitializeSubscribersAsync();
        };
        s_mqttClient.ApplicationMessageReceivedAsync += HandleMessageReceivedAsync;

        try
        {
            if (!s_mqttClient.IsConnected)
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithClientId(_brokerConfig.ClientSettings.ClientId)
                    .WithConnectionUri(_brokerConfig.Server)
                    .WithCredentials(_brokerConfig.ClientSettings.Username, _brokerConfig.ClientSettings.Password)
                    .WithCleanSession()
                    .Build();

                _ = await s_mqttClient.ConnectAsync(mqttClientOptions, cancellationToken);
            }
        }
        catch (MqttConnectingFailedException connEx)
        {
            _logger.LogError(connEx, "Failed to connect to MQTT broker");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to start consuming events");
            throw;
        }

        async Task InitializeSubscribersAsync()
        {
            var mqttSubscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(f => f
                    .WithTopic(_brokerConfig.Topics[0])
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce))
                .Build();

            var subscribeResult = await s_mqttClient.SubscribeAsync(mqttSubscribeOptions, cancellationToken);
            if (subscribeResult.IsSuccess())
            {
                _logger.LogDebug(
                    "Subscribed to topics: {TopicsName}",
                    string.Join(',', subscribeResult.GetSubscribedTopics()));
                return;
            }

            _logger.LogError("Failed to subscribe to topics");
        }

        async Task HandleMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
        {
            var payload = args.ApplicationMessage.ConvertPayloadToString();
            var topic = args.ApplicationMessage.Topic;
            
            _logger.LogDebug(
                "Received message from topic {Topic} with payload {Payload}",
                topic, payload);
            
            try
            {
                var eventType = topic switch
                {
                    var t when t.StartsWith("speed-cam/") => TrafficEventType.VehicleDetected,
                    _ => throw new ArgumentOutOfRangeException()
                };

                await messageReceivedHandler(payload, eventType, cancellationToken);
                
                args.IsHandled = true;
                await args.AcknowledgeAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                args.ProcessingFailed = true;
                args.ReasonCode = MqttApplicationMessageReceivedReasonCode.UnspecifiedError;
                args.ResponseReasonString = ex.Message;
                _logger.LogError(ex, "Failed to process message from topic {Topic}", topic);
                _logger.LogDebug("Message payload: {Payload}", payload);
            }
        }
    }

    public async Task StopConsumeAsync(CancellationToken cancellationToken)
    {
        if (s_mqttClient.IsConnected)
        {
            var mqttUnsubscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
                .WithTopicFilter(_brokerConfig.Topics[0])
                .Build();

            var unsubscribeResult = await s_mqttClient.UnsubscribeAsync(mqttUnsubscribeOptions,cancellationToken);
            if (unsubscribeResult.Items.All(i => i.ResultCode is MqttClientUnsubscribeResultCode.Success))
            {
                _logger.LogDebug("Unsubscribed from topics: {TopicsName}",
                    string.Join(',', unsubscribeResult.Items.Select(i => i.TopicFilter)));

                s_mqttClient.DisconnectedAsync += async (args) =>
                {
                    _logger.LogDebug("Disconnected from MQTT broker with reason {Reason}", args.ReasonString);
                    await DisposeAsync();
                };
                
                var mqttDisconnectOptions = new MqttClientDisconnectOptionsBuilder()
                    .WithReason(MqttClientDisconnectReason.NormalDisconnection)
                    .WithReasonString($"Disconnecting consumer client {_brokerConfig.ClientSettings.ClientId}")
                    .Build();
                
                await s_mqttClient.DisconnectAsync(mqttDisconnectOptions, cancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        s_mqttClient.Dispose();
        await ValueTask.CompletedTask;
    }
}