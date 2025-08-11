using Newtonsoft.Json;

namespace WSocket.Messaging.Messages.Responses;

/// <summary>
/// Ответ на запрос присоединиться к группе.
/// Этот объект будет сериализован в строку и отправлен на клиенту.
/// </summary>
public class JoinGroupResponse
{
    [JsonProperty(PropertyName = "groupId")]
    public required string GroupId { get; set; }

    [JsonProperty(PropertyName = "messageId")]
    public string? MessageId { get; set; } // клиент может прислать id сообщения, чтобы гарантированно идентифицировать ответ
}