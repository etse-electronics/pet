using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace webapi.WebSockets;

public class DeviceWebSocketMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, DeviceStateService deviceState)
    {
        Console.WriteLine($"WebSocket request: {context.Request.Path}");
        if (context.Request.Path == "/api/ws/devices")
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var ws = await context.WebSockets.AcceptWebSocketAsync();
                deviceState.AddClient(ws);

                await deviceState.Listen(ws); // keep alive
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        }
        else
        {
            await next(context);
        }
    }
}