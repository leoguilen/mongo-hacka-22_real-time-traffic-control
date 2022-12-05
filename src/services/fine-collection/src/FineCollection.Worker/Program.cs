var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services
            .AddOptions<AmqpBrokerConfig>()
            .BindConfiguration("AmqpBroker")
            .ValidateOnStart()
            .Services
            .AddSingleton<IModel>(sp =>
            {
                var config = sp.GetRequiredService<IOptions<AmqpBrokerConfig>>().Value;
                var connectionFactory = new ConnectionFactory
                {
                    HostName = config.Server,
                    UserName = config.Username,
                    Password = config.Password,
                    ClientProvidedName = config.ClientId,
                    AutomaticRecoveryEnabled = true,
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
                };
                
                var connection = connectionFactory.CreateConnection();
                var channel = connection.CreateModel();
                
                return channel;
            })
            .AddSingleton<IMongoCollection<TrafficViolationDocument>>(_ =>
            {
                var connectionString = ctx.Configuration.GetConnectionString("MongoDb");
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase("TRAFFIC_CONTROL");
                return database.GetCollection<TrafficViolationDocument>("TRAFFIC_VIOLATIONS");
            })
            .AddSingleton<IMessageHandler, RabbitMqMessageHandler>()
            .AddSingleton<ITrafficViolationRepository, TrafficViolationMongoRepository>()
            .AddHttpClient<IVehicleRegistrationApiClient, VehicleRegistrationApiHttpClient>()
            .ConfigureHttpClient((_, client) =>
            {
                var config = ctx.Configuration.GetSection("VehicleRegistrationApi");
                client.BaseAddress = config.GetValue<Uri>("BaseUrl");
                client.Timeout = TimeSpan.FromMilliseconds(config.GetValue("TimeoutInMs", 0));
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "FineCollection.Worker");
            })
            .Services
            .AddSingleton<ITrafficViolationProcessor, TrafficViolationProcessor>()
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();