using System.Text;

namespace WSocket.Utils;

public static class Converter
{
    /// <summary>
    /// Операции с массивом байт:
    ///    - получить подмассив из массива;
    ///    - сдвинуть массив -> (вправо) на переданное кол-во байт.
    /// </summary>
    /// <param name="srcArr">Исходный массив байт.</param>
    /// <param name="skip">Пропустить от начала столько бит.</param>
    /// <param name="take">Взять столько бит, после пропуска.</param>
    /// <param name="resultStartOffset">Сдвиг вправо. В результирующем массиве сделать отступ в начале на столько байт.</param>
    public static byte[] GetRangeBytes(byte[] srcArr, int skip = 0, int take = 0, int resultStartOffset = 0)
    {
        var resArr = new byte[resultStartOffset + take];
        Array.Copy(srcArr, skip, resArr, resultStartOffset, take);
        return resArr;
    }

    /// <summary>
    /// Получить строку из массива байт.
    /// Целевой массив байт может быть подмассивом исходного.
    /// У целевого массива могут быть на конце пустые значения, которые надо обрезать в результирующей строке.
    /// </summary>
    /// <param name="srcArr">Исходный массив байт.</param>
    /// <param name="skip">Пропустить от начала столько бит.</param>
    /// <param name="take">Взять столько бит, после пропуска.</param>
    /// <param name="resultStartOffset">В результирующем массиве сделать отступ вначале на такое количество байт</param>
    public static string GetString(byte[] srcArr, int skip = 0, int take = 0, int resultStartOffset = 0)
    {
        var strBytes = GetRangeBytes(srcArr, skip, take, resultStartOffset);
        var str = Encoding.UTF8.GetString(strBytes);
        return str.TrimEnd('\0');
    }

    /// <summary>
    /// Конвертирует два байта в ushort.
    /// Используется порядок big-endian.
    /// Например.
    /// Пришло два байта:
    ///    byte1 = 00000001 = 1
    ///    byte2 = 00000010 = 2
    /// присоединим к byte1 справа byte2, получим:
    ///    0000000100000010 = 258
    /// </summary>
    public static ushort BytesToUshort(byte byte1, byte byte2)
    {
        return (ushort)((byte1 << 8) | byte2);
    }

    /// <summary>
    /// Конвертирует ushort в два байта.
    /// Используется порядок big-endian.
    /// Например.
    /// Пришло число 65000, в двоичном представлении: 1111110111101000
    ///   byte1 = 11111101 = 253
    ///   byte2 = 11101000 = 232
    /// </summary>
    public static byte[] UshortToBytes(ushort value)
    {
        byte byte1 = 0;
        byte byte2 = 0;

        for (var i = 15; i >= 8; i--)
        {
            if ((value & (1 << i)) == 0)
                continue;
            var num = (byte)(1 << (i - 8));
            byte1 |= num;
        }

        for (var i = 7; i >= 0; i--)
        {
            if ((value & (1 << i)) == 0)
                continue;
            var num = (byte)(1 << i);
            byte2 |= num;
        }

        return [byte1, byte2];
    }
}