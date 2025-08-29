using webapi.Repository;

namespace webapi.WebSockets;

public class DeviceBackgroundService(DeviceStateService deviceState, IDeviceRepository repository) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // get all devices, might be more specific here
            var devices = await repository.GetDevices();

            deviceState.BroadcastAsync(devices);

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}
