using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SatialInterfaces.Helpers;

/// <summary>This class contains IEnumerable methods.</summary>
internal static class EnumerableHelper
{
	/// <summary>
	/// Determines whether a sequence contains any elements.
	/// </summary>
	/// <param name="source">The IEnumerable to check for emptiness.</param>
	/// <returns>true if the source sequence contains any elements; otherwise, false.</returns>
	public static bool Any(this IEnumerable source) => source.GetEnumerator().MoveNext();

	/// <summary>
	/// Creates a list with objects from an enumerable
	/// </summary>
	/// <param name="source">The IEnumerable to convert.</param>
	/// <returns>The list</returns>
	public static List<object> ToList(this IEnumerable source) => Enumerable.ToList(source.Cast<object>());
}