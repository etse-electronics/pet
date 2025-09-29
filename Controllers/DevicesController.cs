using Microsoft.AspNetCore.Mvc;
using webapi.Repository;

namespace webapi.Controllers;

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

    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        var devices = await deviceRepository.GetDevices();
        return Ok(devices);
    }
}