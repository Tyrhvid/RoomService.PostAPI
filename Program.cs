using System.Text.Json;
using RoomService.PostAPI.Entities;
using RoomService.PostAPI.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:4951");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<RabbitMqService>();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

builder.Services
    .AddCors(options =>
        options.AddPolicy(
            "TyrSecretRoom",
            policy =>
                policy
                    .WithOrigins("https://localhost:7777")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
        )
    );

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("TyrSecretRoom");

app.MapMethods("/rooms", new[] { "OPTIONS" }, (HttpContext context) =>
{
    context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:7777");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, OPTIONS");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
    return Results.Ok();
});


// Endpoint för pots
app.MapPost("/rooms", async (Room room, RabbitMqService rabbitMqService) =>
{
    try
    {
        var jsonRoom = JsonSerializer.Serialize(room);
        rabbitMqService.PublishMessage(jsonRoom);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.StatusCode(500);
    }
});

app.Run();