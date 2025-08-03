using WSocket.Utils;

namespace WSocket.Messaging.Groups;

public abstract class Group : IDisposable
{
    protected ConcurrentHashSet<ClientConnection> ClientConnections { get; } = [];


    #region Клиентские соединения

    public bool AddConnection(ClientConnection connection) => ClientConnections.Add(connection);
    public bool RemoveConnection(ClientConnection connection) => ClientConnections.Remove(connection);

    public void DisposeConnection(ClientConnection connection)
    {
        connection.Dispose();
        RemoveConnection(connection);
    }

    #endregion Клиентские соединения


    public void Dispose()
    {
        foreach (var (connection, _) in ClientConnections)
            DisposeConnection(connection);
    }
}