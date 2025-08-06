using System.Collections.Concurrent;

namespace WSocket.Utils;

/// <summary>
/// Конкурентное множество.
/// Сделано на основе ConcurrentDictionary.
/// </summary>
public class ConcurrentHashSet<T> : ConcurrentDictionary<T, byte>
    where T : notnull
{
    const byte DummyByte = byte.MinValue;

    public bool Contains(T item) => ContainsKey(item);
    public bool Add(T item) => TryAdd(item, DummyByte);
    public bool Remove(T item) => TryRemove(item, out _);
}