using WSocket.Contracts;
using WSocket.Utils;

namespace WSocket.Messaging.Messages;

/// <summary>
/// Сообщение, связанное с группой:
///   - запрос на присоединение к группе;
///   - и т.п.
///
/// Сообщение для взаимодействия/управления группой:
/// 
///     2 байта           2 байта        Остальные байты 
///    -------------------------------------------------
///   |  ТипСообщения  |  ТипГруппы  |      Данные     |
///   -------------------------------------------------
///
/// Сообщение для членов группы:
/// 
///     2 байта           50 байт      Остальные байты 
///    -------------------------------------------------
///   |  ТипСообщения  |  ИдГруппы  |      Данные     |
///   -------------------------------------------------
/// 
/// </summary>
public class MessageGroupBased : Message
{
    public GroupType GroupType { get; }

    public MessageGroupBased(MessageType messageType, GroupType groupType, byte[] raw)
        : base(messageType, raw, Defaults.MessageTypeBytesLength + Defaults.GroupTypeBytesLength)
    {
        GroupType = groupType;
    }


    public static GroupType ParseGroupType(byte[] bytes)
    {
        var groupTypeUshort = Converter.BytesToUshort(bytes[0], bytes[1]);
        return (GroupType)groupTypeUshort;
    }

    public static string ParseGroupId(byte[] bytes)
    {
        var skipCount = Defaults.MessageTypeBytesLength;
        var takeCount = Defaults.GroupIdBytesLength;
        return Converter.GetString(bytes, skipCount, takeCount);
    }
}