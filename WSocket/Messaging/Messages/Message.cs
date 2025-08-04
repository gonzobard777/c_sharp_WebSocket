using System.Text;
using WSocket.Contracts;

namespace WSocket.Messaging.Messages;

public class Message
{
    // Тип сообщения присутствует в заголовке каждого сообщения.
    public readonly MessageType Type;

    // Полное исходное сообщение в том виде как оно пришло из сокета.
    public byte[] Raw { get; set; }

    // Длина заголовка, в штуках байт.
    // Один заголовок может состоять из нескольких блоков.
    // Например:
    //   - заголовок сообщения, чтобы "Войти в группу" состоит из: ТипСообщения, ТипГруппы.
    //   - заголовок сообщения, предназначенное для членов группы состоит из: ТипСообщения, ИдГруппы.
    public readonly int HeaderLength;

    // "Сырые" данные - это строка, в которую десериализован JSON-объект.
    private string _dataRaw = "";

    public Message()
    {
    }

    public Message(MessageType messageType, byte[] raw, int headerLength = 1)
    {
        Type = messageType;
        Raw = raw;
        HeaderLength = headerLength;
    }

    /// <summary>
    /// Используется для отправки сообщения на клиент.
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="content">Данные</param>
    public Message(MessageType messageType, string content)
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);
        Raw = new byte[1 + contentBytes.Length];
        Raw[0] = (byte)messageType;
        Array.Copy(contentBytes, 0, Raw, 1, contentBytes.Length);
    }

    /**
     * Для получения "сырых" данных надо сконвертировать массив байт в строку.
     * Конвертация производится только по запросу и один раз.
     */
    public string DataRaw
    {
        get
        {
            if (_dataRaw == "" && HeaderLength < Raw.Length)
                // Все, что не заголовок, является данными.
                _dataRaw = Encoding.UTF8.GetString(Raw, HeaderLength, Raw.Length - HeaderLength);
            return _dataRaw;
        }
    }
}