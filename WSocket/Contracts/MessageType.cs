namespace WSocket.Contracts;

public enum MessageType : byte
{
    // Войти в группу.
    JoinGroup = 0,
    JoinGroupResponse = 1,

    // Сообщение для членов группы.
    GroupMessage = 2, 
}