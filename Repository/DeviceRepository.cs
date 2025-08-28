using Dapper;
using webapi.Repository.Entities;

namespace webapi.Repository;

public interface IDeviceRepository
{
    Task<Device> Seen(string id);
    Task<Device?> GetDevice(string id);
    Task<IEnumerable<Device>> GetDevices();
}

public class DeviceRepository(DataContext context) : IDeviceRepository
{
    public async Task<Device> Seen(string id)
    {
        using var connection = context.CreateConnection();
        var sql = """
            INSERT INTO Devices (Id, LastSeen)
            VALUES (@Id, @LastSeen)
            ON CONFLICT(Id) DO UPDATE 
                SET LastSeen = excluded.LastSeen
            RETURNING *;
            """;

        return await connection.QuerySingleAsync<Device>(
            sql,
            new { Id = id, LastSeen = DateTime.UtcNow }
        );
    }

    public async Task<Device?> GetDevice(string id)
    {
        using var connection = context.CreateConnection();
        var sql = "SELECT * FROM Devices WHERE Id = @Id";
        return await connection.QuerySingleOrDefaultAsync<Device>(sql, new { Id = id });
    }

    public async Task<IEnumerable<Device>> GetDevices()
    {
        using var connection = context.CreateConnection();
        var sql = "SELECT * FROM Devices";
        return await connection.QueryAsync<Device>(sql);
    }
}