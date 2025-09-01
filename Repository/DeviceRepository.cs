using System.Data;
using Dapper;
using webapi.Repository.Entities;

namespace webapi.Repository;

public interface IDeviceRepository
{
    Task<Device> Seen(string id, int state);
    Task<Device?> GetDevice(string id);
    Task<IEnumerable<Device>> GetDevices();
    Task Reset();
}

public class DeviceRepository(DataContext context) : IDeviceRepository
{
    public async Task<Device> Seen(string id, int state)
    {
        using var connection = context.CreateConnection();
        var sql = """
            INSERT INTO Devices (Id, State, LastSeen)
            VALUES (@Id, @State, @LastSeen)
            ON CONFLICT(Id) DO UPDATE 
                SET LastSeen = excluded.LastSeen
            RETURNING *;
            """;

        return await connection.QuerySingleAsync<Device>(
            sql,
            new { Id = id, State = state, LastSeen = DateTime.UtcNow }
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

    public async Task Reset()
    {
        using var connection = context.CreateConnection();
        var sql = """
            DELETE FROM Logs;
            DELETE FROM Devices;
        """;
        await connection.ExecuteAsync(sql);
    }
}