using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace webapi.Repository;

public class DataContext(IConfiguration configuration)
{    
    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(configuration.GetConnectionString("SqliteDb"));
    }

    public async Task Init()
    {        
        using var connection = CreateConnection();
        await _initDevices();

        async Task _initDevices()
        {
            var sql = """
            CREATE TABLE IF NOT EXISTS Devices (
                Id TEXT NOT NULL PRIMARY KEY,
                State INTEGER NOT NULL DEFAULT 0,
                LastSeen DATETIME
            );

            CREATE TABLE IF NOT EXISTS Logs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DeviceId TEXT NOT NULL,
                Topic TEXT NOT NULL,
                Payload TEXT NOT NULL,
                Created DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (DeviceId) REFERENCES Devices(Id)
            );
            """;
            await connection.ExecuteAsync(sql);
        }
    }
}