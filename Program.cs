var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

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
