using MQTTnet;
using webapi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddSingleton(sp =>
{
    var factory = new MqttClientFactory();
    return factory.CreateMqttClient();
});
builder.Services.AddHostedService<MqttSubscribe>();

var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
