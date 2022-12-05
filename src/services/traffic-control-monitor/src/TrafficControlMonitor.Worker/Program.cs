var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services
            .AddOptions<MqttBrokerConfig>()
            .BindConfiguration("MqttBroker")
            .ValidateOnStart()
            .Services
            .AddSingleton<IValidateOptions<MqttBrokerConfig>, MqttBrokerConfigValidation>()
            .AddSingleton<IEventBus, MosquittoEventBus>()
            .AddSingleton<IMongoCollection<VehicleDetectedEventDocument>>(sp =>
            {
                var mongoClient = new MongoClient(ctx.Configuration.GetConnectionString("MongoDb"));
                var database = mongoClient.GetDatabase("TRAFFIC_CONTROL");
                return database.GetCollection<VehicleDetectedEventDocument>("VEHICLE_DETECTED_EVENTS");
            })
            .AddSingleton<IVehicleDetectedEventRepository, VehicleDetectedEventMongoRepository>()
            .AddSingleton<IModel>(sp =>
            {
                var factory = new ConnectionFactory
                {
                    HostName = ctx.Configuration["RabbitMq:HostName"],
                    UserName = ctx.Configuration["RabbitMq:UserName"],
                    Password = ctx.Configuration["RabbitMq:Password"],
                    VirtualHost = ctx.Configuration["RabbitMq:VirtualHost"] ?? "/"
                };
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();
                
                _ = channel.QueueDeclare(
                    queue: ctx.Configuration["RabbitMq:QueueName"],
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
                
                return channel;
            })
            .AddTransient<VehicleDetectedEventHandler>()
            .AddSingleton<Func<TrafficEventType, Expression<Func<IEvent, CancellationToken, Task>>>>(sp => type =>
                type switch
                {
                    TrafficEventType.VehicleDetected => (@event, cts) =>
                        sp.GetService<VehicleDetectedEventHandler>()!.HandleAsync((VehicleDetectedEvent) @event, cts),
                    _ => throw new KeyNotFoundException($"No handler found for traffic event type {type.ToString()}")
                })
            .AddSingleton<ITrafficEventIngestionService, TrafficEventIngestionService>()
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();