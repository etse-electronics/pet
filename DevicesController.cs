using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using webapi.Repository;

namespace webapi;

[ApiController]
[Route("api/[controller]")]
public class DevicesController(IDeviceRepository deviceRepository) : ControllerBase
{
    [HttpGet()]
    public string Get()
    {
        return "devicecontroller is ready";
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        await deviceRepository.Reset();
        return Ok();
    }
}