namespace webapi.Repository.Entities;

public class Device
{
    public required string Id { get; set; }
    public int State { get; set; }
    public DateTime LastSeen { get; set; }
}