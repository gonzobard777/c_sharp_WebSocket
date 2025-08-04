namespace WSocket.Contracts;

public enum MessageType : byte
{
    // Взаимодействие с группой.
    JoinGroup = 0,
    JoinGroupResponse = 1,
    LeaveGroup = 2,

    // Сообщение для членов группы.
    GroupMessage = 3,
}