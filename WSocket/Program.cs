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
        var appLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        var messageHandler = app.Services.GetRequiredService<MessageHandler>();
        appLifetime.ApplicationStopping.Register(() => messageHandler.Dispose());
        // Вебсокет должен быть определен до UseEndpoints.
        var webSocketOptions = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromMinutes(2) };
        app.UseWebSockets(webSocketOptions);

        app.MapControllers();

        app.Run();
    }
}