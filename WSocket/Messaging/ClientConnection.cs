using System.Collections.Concurrent;
using System.Net.WebSockets;
using WSocket.Messaging.Messages;

namespace WSocket.Messaging;

public class ClientConnection : IDisposable
{
    public WebSocket WebSocket { get; }
    private ConcurrentQueue<Message> QueueToSend { get; } = [];
    private MessageHandler MessageHandler { get; }

    public ClientConnection(WebSocket webSocket, MessageHandler messageHandler)
    {
        WebSocket = webSocket;
        MessageHandler = messageHandler;
    }

    public async Task Run(CancellationToken cancellationToken)
    {
        // Отправить.
        var sending = Task.Run(async () => await Sending(cancellationToken), cancellationToken);

        // Получить.
        try
        {
            while (WebSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var messageBytes = await Read(cancellationToken);
                if (messageBytes is { Length: > 0 })
                    MessageHandler.Run(messageBytes, this, cancellationToken);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        await sending;
    }

    /// <summary>
    /// Добавить новое сообщение в очередь для отправки.
    /// </summary>
    public void Send(Message message)
    {
        if (WebSocket.State == WebSocketState.Open)
            QueueToSend.Enqueue(message);
    }

    /// <summary>
    /// Процесс отправки сообщений из очереди.
    /// </summary>
    private async Task Sending(CancellationToken cancellationToken)
    {
        while (WebSocket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
        {
            if (QueueToSend.TryDequeue(out var message))
                await WebSocket.SendAsync(message.Raw, WebSocketMessageType.Binary, true, cancellationToken);

            if (QueueToSend.IsEmpty)
                await Task.Delay(500, cancellationToken);
        }
    }

    public async Task<byte[]?> Read(CancellationToken cancellationToken)
    {
        WebSocketReceiveResult received;
        var chunkBuffer = new ArraySegment<byte>(new byte[1024 * 5]); // 5KB(Kilobyte)
        using var memoryBuffer = new MemoryStream(chunkBuffer.Count);
        do
        {
            received = await WebSocket.ReceiveAsync(chunkBuffer, cancellationToken);
            if (received.CloseStatus.HasValue)
                return null;

            if (received.Count > 0)
            {
                memoryBuffer.Write(chunkBuffer.Array!, 0, received.Count);
            }
        } while (!received.EndOfMessage && !cancellationToken.IsCancellationRequested);

        return memoryBuffer.ToArray();
    }


    public void Dispose()
    {
        try
        {
            WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        QueueToSend.Clear();
    }
}