namespace FineCollection.Worker.Configs;

internal class AmqpBrokerConfigValidation : IValidateOptions<AmqpBrokerConfig>
{
    public ValidateOptionsResult Validate(string name, AmqpBrokerConfig options)
    {
        if (!Uri.TryCreate(options.Server, UriKind.Absolute, out _))
        {
            return ValidateOptionsResult.Fail("Invalid AMQP broker server address");
        }

        return options switch
        {
            {Username: null or ""} => ValidateOptionsResult.Fail("Invalid AMQP broker username"),
            {Password: null or ""} => ValidateOptionsResult.Fail("Invalid AMQP broker password"),
            {QueueName: null or ""} => ValidateOptionsResult.Fail("Invalid AMQP broker topic"),
            _ => ValidateOptionsResult.Success
        };
    }
}