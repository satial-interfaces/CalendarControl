using System.Collections;

namespace Mercury.View;

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
}