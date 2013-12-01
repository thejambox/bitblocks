using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GenericExtensions
{
    public static void Populate<T>(this T[] arr, T value)
    {
        for (int i = 0; i < arr.Length; ++i)
        {
            arr[i] = value;
        }
    }

    public static void AddExclusive<T>(this IList<T> list, T data)
    {
        if (!list.Contains(data))
            list.Add(data);
    }

    /// <summary>
    /// Get a random element from the list.
    /// </summary>
    public static T GetRandom<T>(this IList<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    /// <summary>
    /// Remove and return a random element from the list.
    /// </summary>
    public static T PopRandom<T>(this IList<T> list)
    {
        var i = Random.Range(0, list.Count);
        var o = list[i];
        list.RemoveAt(i);
        return o;
    }

    /// <summary>
    /// Remove and return a element in the list by index. Out of range indices are wrapped into range.
    /// </summary>
    public static T Pop<T>(this IList<T> list, int index)
    {
        while (index > list.Count) index -= list.Count;
        while (index < 0) index += list.Count;
        var o = list[index];
        list.RemoveAt(index);
        return o;
    }

    /// <summary>
    /// Return an element from a list by index. Out of range indices are wrapped into range.
    /// </summary>
    public static T Get<T>(this IList<T> list, int index)
    {
        while (index > list.Count) index -= list.Count;
        while (index < 0) index += list.Count;
        return list[index];
    }

    /// <summary>
    /// Get the last item in the list. Returns null for empty lists.
    /// </summary>
    public static T GetLast<T>(this IList<T> list)
    {
        if (list.Count == 0)
            return default(T);

        return list[list.Count - 1];
    }

    /// <summary>
    /// Remove and return a value from a dictionary.
    /// </summary>
    public static TValue Pop<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
    {
        var item = dict[key];
        dict.Remove(key);
        return item;
    }

    /// <summary>
    /// Map a function onto an entire list of items.
    /// </summary>
    public static void Map<T>(this IList<T> list, System.Action<T> fn)
    {
        for (var i = 0; i < list.Count; i++)
        {
            fn(list[i]);
        }
    }

    /// <summary>
    /// Apply a function onto an entire list of items, reassigning the result to the current item.
    /// </summary>
    public static void Apply<T>(this IList<T> list, System.Func<T, T> fn)
    {
        for (var i = 0; i < list.Count; i++)
        {
            list[i] = fn(list[i]);
        }
    }
}
