using System.Collections.Concurrent;
using WSocket.Contracts;
using WSocket.Messaging.Groups;
using WSocket.Messaging.Messages;

namespace WSocket.Messaging;

public class MessageHandler : IDisposable
{
    private readonly ConcurrentDictionary<string, Group> Groups = new();

    /// <summary>
    /// Обработка нового сообщения.
    /// </summary>
    /// <param name="bytes">Полное исходное сообщение в том виде как оно пришло из сокета.</param>
    /// <param name="connection">Клиентское соединение, из которого пришло сообщение.</param>
    /// <param name="cancellationToken"></param>
    public async Task Run(byte[] bytes, ClientConnection connection, CancellationToken cancellationToken)
    {
        var messageType = (MessageType)bytes[0];
        switch (messageType)
        {
            // Найти/создать группу. Добавить туда пользователя.
            case MessageType.JoinGroup:
            {
                // 1. Определить groupId. По типу группы и доп.данным.
                var groupId = "";
                Func<string, Group>? groupCreator = null;

                var groupType = (GroupType)bytes[1];
                switch (groupType)
                {
                    case GroupType.CursorMoving:
                        var parsedMessage = new CursorMovingMessageJoinGroup(messageType, groupType, bytes);
                        groupId = CursorMovingGroup.GenerateId(parsedMessage);
                        groupCreator = _ => new CursorMovingGroup();
                        break;
                }

                if (groupId.Length > 0 && groupCreator != null)
                {
                    // 2. Найти/создать группу.
                    var group = Groups.GetOrAdd(groupId, groupCreator);

                    // 3. Добавить пользователя в группу.
                    if (group.AddConnection(connection))
                    {
                    }
                }

                break;
            }


            case MessageType.GroupMessage:
            {
                break;
            }
        }
    }


    public void DisposeClientConnection(ClientConnection connection)
    {
        foreach (var (_, group) in Groups)
            group.DisposeConnection(connection);
    }


    public void Dispose()
    {
        foreach (var (groupId, group) in Groups)
        {
            group.Dispose();
            Groups.TryRemove(groupId, out _);
        }
    }
}