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
        var messageType = Message.ParseType(bytes);
        switch (messageType)
        {
            // Найти/создать группу. Добавить в нее пользователя.
            case MessageType.JoinGroup:
            {
                // 1. Определить groupId. По типу группы и доп.данным.
                var groupId = "";
                Func<string, Group>? groupCreator = null;

                var groupType = MessageGroupBased.ParseGroupType(bytes);
                switch (groupType)
                {
                    case GroupType.CursorMoving:
                        var parsedMessage = new CursorMovingMessageJoinGroup(messageType, groupType, bytes);
                        groupId = CursorMovingGroup.CreateId(parsedMessage);
                        groupCreator = _ => new CursorMovingGroup();
                        break;
                }

                if (groupId.Length > 0 && groupCreator != null)
                {
                    // 2. Найти/создать группу.
                    var group = Groups.GetOrAdd(groupId, groupCreator);

                    // 3. Добавить пользователя в группу.
                    group.AddConnection(connection);

                    // 4. Отправить результат клиенту.
                    connection.Send(new Message(MessageType.JoinGroupResponse, groupId));
                }

                break;
            }


            case MessageType.GroupMessage:
            {
                var groupId = MessageGroupBased.ParseGroupId(bytes);
                Groups.TryGetValue(groupId, out var group);
                if (group == null) return;
                connection.Send(new Message { Raw = bytes });
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