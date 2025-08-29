using MQTTnet;
using webapi;
using webapi.Repository;
using webapi.WebSockets;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddSingleton(services =>
{
    var factory = new MqttClientFactory();
    return factory.CreateMqttClient();
});
builder.Services.AddHostedService<MqttSubscribe>();

builder.Services.AddSingleton<DataContext>();
builder.Services.AddSingleton<IDeviceRepository, DeviceRepository>();
builder.Services.AddSingleton<ILogRepository, LogRepository>();

// WebSockets
builder.Services.AddSingleton<DeviceStateService>();
builder.Services.AddHostedService<DeviceBackgroundService>();

var app = builder.Build();
app.MapControllers();

app.UseWebSockets();
app.UseMiddleware<DeviceWebSocketMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// create database and tables
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    await context.Init();
}

app.UsePathBase("/api");

app.MapGet("/", () =>
{
    return "service is running";
});

app.MapGet("/ping", () =>
{
    return "pong";
})
.WithName("Ping");

app.Run();
