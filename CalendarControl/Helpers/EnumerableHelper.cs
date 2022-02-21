using System.Collections;
using System.Collections.Generic;

namespace CalendarControl.Helpers;

/// <summary>
/// This class contains IEnumerable methods.
/// </summary>
internal static class EnumerableHelper
{
    /// <summary>
    /// Determines whether a sequence contains any elements.
    /// </summary>
    /// <param name="source">The IEnumerable to check for emptiness.</param>
    /// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
    public static bool Any(this IEnumerable source)
    {
        var enumerator = source.GetEnumerator();
        return enumerator.MoveNext();
    }
    /// <summary>
    /// Creates a list with objects from an enumerable
    /// </summary>
    /// <param name="source">The IEnumerable to convert.</param>
    /// <returns>The list</returns>
    public static IList<object> ToList(this IEnumerable source)
    {
        var result = new List<object>();
        foreach (var obj in source)
            result.Add(obj);
        return result;
    }
}
