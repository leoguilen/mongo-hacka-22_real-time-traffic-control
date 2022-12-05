namespace TrafficControlMonitor.Worker.Configs;

internal class MqttBrokerConfigValidation : IValidateOptions<MqttBrokerConfig>
{
    public ValidateOptionsResult Validate(string name, MqttBrokerConfig options)
    {
        if (options.Server is null || !Uri.TryCreate(options.Server, UriKind.Absolute, out _))
        {
            return ValidateOptionsResult.Fail("Invalid MQTT broker server address");
        }

        if (options.Topics is null or {Length: 0})
        {
            return ValidateOptionsResult.Fail("Invalid MQTT broker topics");
        }

        if (options.ClientSettings is null or {ClientId: null} or {Password: null} or {Username: null})
        {
            return ValidateOptionsResult.Fail("Invalid MQTT broker client settings");
        }

        return ValidateOptionsResult.Success;
    }
}