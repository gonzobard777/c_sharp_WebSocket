using WSocket.Messaging;

namespace WSocket;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddSingleton<MessageHandler>();


        // Configure the HTTP request pipeline.
        var app = builder.Build();

        var webSocketOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromMinutes(2) };
        app.UseWebSockets(webSocketOptions);

        app.MapControllers();

        app.Run();
    }
}