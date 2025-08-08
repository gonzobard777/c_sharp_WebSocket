using WSocket.Messaging.Messages;
using WSocket.Utils;

namespace WSocket.Messaging.Groups;

public abstract class Group : IDisposable
{
    // Члены группы.
    protected ConcurrentHashSet<ClientConnection> ClientConnections { get; } = [];

    /// <summary>
    /// Добавление в очередь сообщений для членов группы.
    /// По умолчанию отправитель сообщения НЕполучает свое же сообщение.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <param name="sender">Отправитель сообщения.</param>
    /// <param name="excludeSender">Исключить отправителя из бродкаста.</param>
    public void Broadcast(Message message, ClientConnection sender, bool excludeSender = true)
    {
        // Параллельностью добавления в очередь высвобождаю текущий тред.
        // Например, чтобы этот тред уже приступил к чтению нового сообщения.
        Parallel.ForEach(ClientConnections, pair =>
        {
            var connection = pair.Key;
            if (excludeSender && connection == sender)
                return;
            connection.Send(message);
        });
    }

    #region Клиентские соединения

    public bool AddConnection(ClientConnection connection) => ClientConnections.Add(connection);
    public bool RemoveConnection(ClientConnection connection) => ClientConnections.Remove(connection);

    #endregion Клиентские соединения


    public void Dispose()
    {
        foreach (var (connection, _) in ClientConnections)
            RemoveConnection(connection);
    }
}