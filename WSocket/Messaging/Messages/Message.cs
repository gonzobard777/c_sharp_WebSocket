using System.Text;
using WSocket.Contracts;
using WSocket.Utils;

namespace WSocket.Messaging.Messages;

/// <summary>
///
///     2 байта           Остальные байты 
///    -----------------------------------
///   |  ТипСообщения  |      Данные     |
///   -----------------------------------
/// 
/// </summary>
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
    public readonly int HeaderLengthBytes;

    // "Сырые" данные - это строка, в которую десериализован JSON-объект.
    private string _dataRaw = "";

    public Message()
    {
    }

    public Message(MessageType messageType, byte[] raw, byte headerLengthBytes = Defaults.MessageTypeBytesLength)
    {
        Type = messageType;
        Raw = raw;
        HeaderLengthBytes = headerLengthBytes;
    }

    /// <summary>
    /// Используется для отправки сообщения на клиент.
    /// </summary>
    /// <param name="messageType">Тип сообщения.</param>
    /// <param name="data">Данные сообщения.</param>
    public Message(MessageType messageType, string data)
    {
        var dataBytes = Encoding.UTF8.GetBytes(data);
        // Сдвинуть массив -> (вправо) на кол-во байт ТипСообщения.
        Raw = Converter.GetRangeBytes(dataBytes, 0, dataBytes.Length, Defaults.MessageTypeBytesLength);
        // Задать значение типа сообщения.
        var typeBytes = GetTypeBytes(messageType);
        Raw[0] = typeBytes[0];
        Raw[1] = typeBytes[1];
    }

    /**
     * Для получения "сырых" данных надо сконвертировать массив байт в строку.
     * Конвертация производится только по запросу и один раз.
     */
    public string DataRaw
    {
        get
        {
            if (_dataRaw == "" && HeaderLengthBytes < Raw.Length)
            {
                // Все, что не заголовок, является данными.
                var skipCount = HeaderLengthBytes;
                var takeCount = Raw.Length - HeaderLengthBytes;
                _dataRaw = Encoding.UTF8.GetString(Raw, skipCount, takeCount);
            }

            return _dataRaw;
        }
    }


    public static MessageType ParseType(byte[] bytes)
    {
        var typeUshort = Converter.BytesToUshort(bytes[0], bytes[1]);
        return (MessageType)typeUshort;
    }

    public static byte[] GetTypeBytes(MessageType messageType)
    {
        return Converter.UshortToBytes((ushort)messageType);
    }
}