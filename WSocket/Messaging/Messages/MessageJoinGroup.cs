using WSocket.Contracts;

namespace WSocket.Messaging.Messages;

/**
 * Сообщение о желании присоединиться к группе.
 */
public class MessageJoinGroup : Message
{
    public GroupType GroupType { get; }

    public MessageJoinGroup(MessageType messageType, GroupType groupType, byte[] raw) : base(messageType, raw, 2)
    {
        GroupType = groupType;
    }
}