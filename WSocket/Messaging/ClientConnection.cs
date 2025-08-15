using System.Collections.Concurrent;
using System.Net.WebSockets;
using WSocket.Messaging.Messages;

namespace WSocket.Messaging;

public class ClientConnection : IDisposable
{
    private WebSocket WebSocket { get; }
    private ConcurrentQueue<Message> QueueToSend { get; } = [];
    private MessageHandler MessageHandler { get; }
    private CancellationToken CancellationToken { get; }

    // Буффер для чтения сообщения из сокета, т.к. заранее неизвестен размер сообщения. 
    private ArraySegment<byte> ChunkBuffer { get; set; } = new(new byte[1024 * 5]); // 5KB(Kilobyte)

    // Время, через которое надо снова проверить QueueToSend, если очередь пуста.
    // Чем выше значение, тем меньше интерактива.
    // Например, стоит задача отображать движение курсоров разных пользователей.
    // Так вот, интервал 500 миллисекунд приведет к тому, что движения курсоров будет более дергано.
    private int SendingQueueCheckIntervalMilliseconds { get; set; } = 7;


    public ClientConnection(WebSocket webSocket, MessageHandler messageHandler, CancellationToken cancellationToken)
    {
        WebSocket = webSocket;
        MessageHandler = messageHandler;
        CancellationToken = cancellationToken;
    }

    public bool IsReady => (
        WebSocket.State == WebSocketState.Open &&
        !CancellationToken.IsCancellationRequested
    );

    public async Task Run()
    {
        // Отправить.
        var sending = Task.Run(async () => await Sending(), CancellationToken);

        // Получить.
        try
        {
            while (IsReady)
            {
                var messageBytes = await Read();
                if (messageBytes is { Length: > 0 })
                    MessageHandler.Run(messageBytes, this);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        await sending;
    }

    /// <summary>
    /// Добавить новое сообщение в очередь отправки.
    /// </summary>
    public void AddToSendQueue(Message message)
    {
        QueueToSend.Enqueue(message);
    }

    /// <summary>
    /// Процесс отправки сообщений из очереди.
    /// </summary>
    private async Task Sending()
    {
        while (IsReady)
        {
            if (QueueToSend.TryDequeue(out var message))
                await WebSocket.SendAsync(message.Raw, WebSocketMessageType.Binary, true, CancellationToken);

            if (QueueToSend.IsEmpty)
                await Task.Delay(SendingQueueCheckIntervalMilliseconds, CancellationToken);
        }
    }

    /// <summary>
    /// Прочитать одно сообщение из сокета:
    ///    1. Повиснуть на await WebSocket.ReceiveAsync пока не придут байты.
    ///    2. Читать байты кусочками в ChunkBuffer.
    ///    3. Собирать результирующее сообщение в memoryBuffer, добавляя к нему новую порцию из ChunkBuffer. 
    /// </summary>
    public async Task<byte[]?> Read()
    {
        WebSocketReceiveResult received;
        using var memoryBuffer = new MemoryStream(ChunkBuffer.Count);
        do
        {
            received = await WebSocket.ReceiveAsync(ChunkBuffer, CancellationToken);
            if (received.CloseStatus.HasValue)
                return null;

            if (received.Count > 0)
            {
                memoryBuffer.Write(ChunkBuffer.Array!, 0, received.Count);
            }
        } while (!received.EndOfMessage && IsReady);

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