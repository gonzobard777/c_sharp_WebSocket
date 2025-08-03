using Newtonsoft.Json;
using WSocket.Contracts;

namespace WSocket.Messaging.Messages;

public class CursorMovingMessageJoinGroup : MessageJoinGroup
{
    public CursorMovingMessageJoinGroup(MessageType messageType, GroupType groupType, byte[] raw) : base(messageType, groupType, raw)
    {
    }

    public CursorMovingData? GetData()
    {
        return JsonConvert.DeserializeObject<CursorMovingData>(DataRaw);
    }
}

public class CursorMovingData
{
    public string Id { get; set; }
}