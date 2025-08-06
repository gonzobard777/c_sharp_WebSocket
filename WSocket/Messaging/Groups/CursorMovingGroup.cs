using WSocket.Messaging.Messages;

namespace WSocket.Messaging.Groups;

public class CursorMovingGroup : Group
{
    public static string CreateId(CursorMovingMessageJoinGroup messageJoin)
    {
        return $"{(ushort)messageJoin.GroupType}-{messageJoin.GetData()?.Id}";
    }
}