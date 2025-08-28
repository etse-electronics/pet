using Dapper;
using webapi.Repository.Entities;

namespace webapi.Repository;

public interface ILogRepository
{
    Task<Log> AddLog(string deviceId, string topic, string payload);
    Task<IEnumerable<Log>> GetLogs(string deviceId);
}

public class LogRepository(DataContext context) : ILogRepository
{
    public async Task<Log> AddLog(string deviceId, string topic, string payload)
    {
        using var connection = context.CreateConnection();
        var sql = """
            INSERT INTO Logs (DeviceId, Topic, Payload)
            VALUES (@DeviceId, @Topic, @Payload)
            RETURNING *;
            """;
        return await connection.QuerySingleAsync<Log>(sql, new { DeviceId = deviceId, Topic = topic, Payload = payload });
    }

    public async Task<IEnumerable<Log>> GetLogs(string deviceId)
    {
        using var connection = context.CreateConnection();
        var sql = "SELECT * FROM Logs WHERE DeviceId = @DeviceId";
        return await connection.QueryAsync<Log>(sql, new { DeviceId = deviceId });
    }
}
