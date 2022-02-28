using System.Collections.Generic;
using SatialInterfaces.Controls;

namespace SatialInterfaces.Helpers;

/// <summary>
/// This class contains appointment group methods
/// </summary>
internal static class AppointmentGroupHelper
{
	/// <summary>
	/// Gets the group count
	/// </summary>
	/// <param name="list">List to get group from</param>
	/// <param name="beginIndex">Index to start from</param>
	/// <returns>The group count</returns>
	public static int GetGroupCount(IList<AppointmentControl> list, int beginIndex)
	{
		var (begin, length) = list[beginIndex].GetFractionOfDay();
		var end = begin + length;
		var count = 1;
		for (var i = beginIndex + 1; i < list.Count; i++)
		{
			var (b, l) = list[i].GetFractionOfDay();
			if (list[i].Indent == 0 && b > end)
				break;

			var e = b + l;
			if (e > end)
				end = e;
			count++;
		}

		return count;
	}

	/// <summary>
	/// Get the indentation count within a group
	/// </summary>
	/// <param name="list">List to search in</param>
	/// <param name="beginIndex">Begin index of group</param>
	/// <param name="count">Count of group</param>
	/// <param name="indent">Indentation to get count for</param>
	/// <returns>List with items matching the indentation</returns>
	public static IList<AppointmentControl> GetIndentationItems(IList<AppointmentControl> list, int beginIndex, int count,
		int indent)
	{
		var result = new List<AppointmentControl>();
		for (var i = beginIndex; i < beginIndex + count; i++)
		{
			if (list[i].Indent == indent)
				result.Add(list[i]);
		}

		return result;
	}

	/// <summary>
	/// Gets the end of the group
	/// </summary>
	/// <param name="list">List to search in</param>
	/// <param name="beginIndex">Begin index of group</param>
	/// <param name="count">Count of group</param>
	/// <returns>The end as a fraction of the day</returns>
	public static double GetEnd(IList<AppointmentControl> list, int beginIndex, int count)
	{
		var (begin, length) = list[beginIndex].GetFractionOfDay();
		var end = begin + length;

		for (var i = beginIndex + 1; i < beginIndex + count; i++)
		{
			var (b, l) = list[i].GetFractionOfDay();
			var e = b + l;
			if (e > end)
				end = e;
		}

		return end;
	}

	/// <summary>
	/// Gets the indentation count for a group
	/// </summary>
	/// <param name="list">List to search in</param>
	/// <param name="beginIndex">Begin index of group</param>
	/// <param name="count">Count of group</param>
	/// <returns>Indentation count</returns>
	public static int GetIndentationCount(IList<AppointmentControl> list, int beginIndex, int count)
	{
		var result = 0;
		for (var i = beginIndex; i < beginIndex + count; i++)
		{
			if (list[i].Indent > result)
				result = list[i].Indent;
		}

		return result + 1;
	}
}