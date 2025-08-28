using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;

namespace webapi;

[ApiController]
[Route("api/[controller]")]
public class MqttController(IMqttClient mqttClient) : ControllerBase
{
    [HttpGet()]
    public string Get()
    {
        return "mqtt is ready";
    }

    [HttpPost("publish")]
    public async Task<IActionResult> Publish([FromBody] PublishRequest request)
    {
        if (!mqttClient.IsConnected)
            return BadRequest("MQTT client not connected");

        var appMessage = new MqttApplicationMessageBuilder()
            .WithTopic(request.topic)
            .WithPayload(JsonSerializer.SerializeToUtf8Bytes(request.message))
            .Build();

        await mqttClient.PublishAsync(appMessage);        
        return Ok("Message published");
    }
}

public record PublishRequest(string topic, dynamic message);