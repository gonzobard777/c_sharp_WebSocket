namespace WSocket.Contracts;

/// <summary>
/// Первые два байта сообщения.
/// Количество возможных типов: 0 to 65535.
/// </summary>
public enum MessageType : ushort
{
    // Взаимодействие с группой.
    JoinGroup = 0,
    JoinGroupResponse = 1,
    LeaveGroup = 2,

    // Сообщение для членов группы.
    GroupMessage = 3,
}