namespace WSocket.Messaging;

public class MessageHandler
{
    private List<ClientConnection> ClientConnections { get; } = [];

    public MessageHandler()
    {
    }

    public async Task Run(Message message, CancellationToken cancellationToken)
    {
        
    }

    #region Клиентские соединения

    public void AddClientConnection(ClientConnection connection)
    {
        lock (ClientConnections)
        {
            ClientConnections.Add(connection);
        }
    }

    public void RemoveClientConnection(ClientConnection connection)
    {
        lock (ClientConnections)
        {
            ClientConnections.Remove(connection);
        }
    }

    #endregion Клиентские соединения
}