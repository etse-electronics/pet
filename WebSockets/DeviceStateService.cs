using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace webapi.WebSockets;

public class DeviceStateService
{
    private readonly List<WebSocket> _clients = [];
    private readonly Lock _lock = new();

    public void AddClient(WebSocket ws)
    {
        lock (_lock) _clients.Add(ws);
    }

    public void BroadcastAsync(object state)
    {
        var json = JsonSerializer.Serialize(state);
        var buffer = Encoding.UTF8.GetBytes(json);

        List<WebSocket> closed = [];
        lock (_lock)
        {
            foreach (var client in _clients)
            {
                if (client.State == WebSocketState.Open)
                {
                    client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    closed.Add(client);
                }
            }

            foreach (var c in closed) _clients.Remove(c);
        }
    }

    public async Task Listen(WebSocket ws)
    {
        var buffer = new byte[1024];
        while (ws.State == WebSocketState.Open)
        {
            try
            {
                var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;
            }
            catch { }
        }
        lock (_lock) _clients.Remove(ws);
    }
}