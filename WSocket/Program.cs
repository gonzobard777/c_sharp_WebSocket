using WSocket.Messaging;

namespace WSocket;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Сервисы и создание приложения.
        builder.Services.AddControllers();
        builder.Services.AddSingleton<MessageHandler>();
        var app = builder.Build();


        // Конфигурирование.
        var webSocketOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromMinutes(2) };
        app.UseWebSockets(webSocketOptions);

        app.MapControllers();

        app.Run();
    }
}