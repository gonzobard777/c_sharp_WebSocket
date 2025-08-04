using System.Text;

namespace WSocket.Utils;

public static class Converter
{
    /// <summary>
    /// Получить подмассив из массива.
    /// Есть возможность сделать отступ с начала массива на заданное количество байт. 
    /// </summary>
    /// <param name="srcArr">Исходный массив байт.</param>
    /// <param name="skip">Пропустить от начала столько бит.</param>
    /// <param name="take">Взять столько бит, после пропуска.</param>
    /// <param name="resultStartOffset">В результирующем массиве сделать отступ в начале на такое количество байт.</param>
    /// <returns></returns>
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
}