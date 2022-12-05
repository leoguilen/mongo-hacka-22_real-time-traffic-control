namespace TrafficControlMonitor.Worker.Infra.EventBus.Mosquitto.Extensions;

internal static class MqttClientSubscribeResultExtensions
{
    public static bool IsSuccess(this MqttClientSubscribeResult result)
        => result.Items
            .All(i => i.ResultCode
                is MqttClientSubscribeResultCode.GrantedQoS0
                or MqttClientSubscribeResultCode.GrantedQoS1
                or MqttClientSubscribeResultCode.GrantedQoS2);

    public static string[] GetSubscribedTopics(this MqttClientSubscribeResult result)
        => result.Items
            .Select(x => x.TopicFilter.Topic)
            .ToArray();
}