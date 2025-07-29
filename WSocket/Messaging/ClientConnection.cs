using System.Collections.Concurrent;
using System.Net.WebSockets;
using WSocket.Contracts;

namespace WSocket.Messaging;

public class ClientConnection(WebSocket webSocket, MessageHandler messageHandler)
{
    public WebSocket WebSocket { get; } = webSocket;
    private ConcurrentQueue<Message> QueueToSend { get; } = [];
    public MessageHandler MessageHandler { get; } = messageHandler;

    public async Task Run(CancellationToken cancellationToken)
    {
        // Отправить.
        var sending = Task.Run(async () => await Sending(cancellationToken), cancellationToken);

        // Получить.
        try
        {
            while (WebSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var message = await Read(cancellationToken);
                if (message != null)
                    MessageHandler.Run(message, cancellationToken);
            }
        }
        catch (Exception e)
        {
            // ignored
        }

        await sending;
    }

    /// <summary>
    /// Процесс отправки сообщений из очереди.
    /// </summary>
    private async Task Sending(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (QueueToSend.TryDequeue(out var message))
            {
                // await _webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, cancellationToken);
            }

            if (QueueToSend.IsEmpty)
                await Task.Delay(500, cancellationToken);
        }
    }

    public async Task<Message?> Read(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        WebSocketReceiveResult received;
        using var messageStream = new MemoryStream();
        do
        {
            received = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            if (received.CloseStatus.HasValue)
            {
                // Если получен статус закрытия, тогда в ответ отправить статус закрытия и остановить чтение.
                await WebSocket.CloseAsync(received.CloseStatus.Value, received.CloseStatusDescription, cancellationToken);
                return null;
            }

            if (received.Count > 0)
            {
                messageStream.Write(buffer, 0, received.Count);
            }
        } while (!received.EndOfMessage && !cancellationToken.IsCancellationRequested);

        var raw = messageStream.ToArray();

        return new Message(MessageType.Message, []);
    }
}