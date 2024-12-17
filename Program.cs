using System.Text.Json;
using RoomService.PostAPI.Entities;
using RoomService.PostAPI.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:4961");

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
                    .WithOrigins("https://localhost:7151", "https://localhost:7173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
        )
    );

var app = builder.Build();

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("TyrSecretRoom");

// Define POST endpoint
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