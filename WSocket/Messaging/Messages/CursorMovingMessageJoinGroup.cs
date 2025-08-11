using Newtonsoft.Json;
using WSocket.Contracts;

namespace WSocket.Messaging.Messages;

public class CursorMovingMessageJoinGroup : MessageGroupBased
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
    public string MessageId { get; set; }
}