namespace webapi.Repository.Entities;

public class Log
{
    public int Id { get; set; }
    public required string DeviceId { get; set; }
    public required string Topic { get; set; }
    public required string Payload { get; set; }
    public DateTime Created { get; set; }
}

