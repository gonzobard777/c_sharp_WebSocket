using WSocket.Messaging.Messages;

namespace WSocket.Messaging.Groups;

public class CursorMovingGroup : Group
{
    public static string GenerateId(CursorMovingMessageJoinGroup message)
    {
        return $"{(byte)message.GroupType}-{message.GetData()?.Id}";
    }
}