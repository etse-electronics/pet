namespace webapi;

using System.Text;
using MQTTnet;
using webapi.Repository;

public class MqttSubscribe(IMqttClient mqttClient, IDeviceRepository deviceRepository, ILogRepository logRepository, IConfiguration configuration) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var server = configuration.GetSection("MqttServer:Server").Value;  // tunnel.etse-tech.com
        var port = Convert.ToInt32(configuration.GetSection("MqttServer:Port").Value); // 1883
        Console.WriteLine("Subscribing to MQTT at {0} on port {1}", server, port);
        var options = new MqttClientOptionsBuilder()
            .WithClientId($"webapi-client-{Guid.NewGuid()}")
            .WithTcpServer(server, port)
            .WithCleanSession()
            .Build();

        mqttClient.ConnectedAsync += async e =>
        {
            Console.WriteLine("CONNECTED to MQTT broker");
            await mqttClient.SubscribeAsync("devices/#", cancellationToken: stoppingToken);
            Console.WriteLine("SUBSCRIBED to devices/#");
        };

        mqttClient.DisconnectedAsync += async e =>
        {
            Console.WriteLine("DISCONNECTED from broker {0}, {1}", e.Reason, e.ReasonString);

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            try
            {
                await mqttClient.ConnectAsync(options, stoppingToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reconnect failed: {ex.Message}");
            }
        };

        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            Console.WriteLine($"Received payload on topic {topic}: {payload}");

            var deviceId = topic.Split("/")[1];
            await deviceRepository.Seen(deviceId);
            await logRepository.AddLog(deviceId, topic, payload);
        };

        await mqttClient.ConnectAsync(options, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
