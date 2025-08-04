using System.Text;
using WSocket.Contracts;
using WSocket.Utils;

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
    /// <param name="data">Данные.</param>
    public Message(MessageType messageType, string data)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data);
        Raw = Converter.GetRangeBytes(dataBytes, 0, dataBytes.Length, 1);
        Raw[0] = (byte)messageType;
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
            {
                // Все, что не заголовок, является данными.
                var skipCount = HeaderLength;
                var takeCount = Raw.Length - HeaderLength;
                _dataRaw = Encoding.UTF8.GetString(Raw, skipCount, takeCount);
            }

            return _dataRaw;
        }
    }
}