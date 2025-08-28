namespace webapi;

using System.Text;
using MQTTnet;

public class MqttSubscribe(IMqttClient mqttClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var options = new MqttClientOptionsBuilder()
            .WithClientId("webapi-client")
            .WithTcpServer("tunnel.etse-tech.com", 1883)
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
            Console.WriteLine("DISCONNECTED from broker");
            
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

        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var topic = e.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            Console.WriteLine($"Received message on topic {topic}: {payload}");

            
            return Task.CompletedTask;
        };

        await mqttClient.ConnectAsync(options, stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}
