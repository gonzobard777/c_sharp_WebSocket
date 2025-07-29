using WSocket.Contracts;

namespace WSocket.Messaging;

public class Message
{
    public MessageType Type { get; }
    public byte[] Data { get; }

    public Message(MessageType type, byte[] data)
    {
        Type = type;
        Data = data;
    }
}