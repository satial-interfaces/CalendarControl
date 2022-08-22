using System.Collections.Generic;
using Avalonia.Controls;
using SatialInterfaces.Controls;

namespace SatialInterfaces.Helpers;

/// <summary>This class contains appointment group methods.</summary>
internal static class AppointmentGroupHelper
{
	/// <summary>
	/// Gets the group count
	/// </summary>
	/// <param name="list">List to get group from</param>
	/// <param name="beginIndex">Index to start from</param>
	/// <returns>The group count</returns>
	public static int GetGroupCount(IList<IControl> list, int beginIndex)
	{
		var item = list[beginIndex].GetFirstLogicalDescendant<IAppointmentControl>();
		var (begin, length) = item.GetFractionOfDay();
		var end = begin + length;
		var count = 1;
		for (var i = beginIndex + 1; i < list.Count; i++)
		{
			item = list[i].GetFirstLogicalDescendant<IAppointmentControl>();
			var (b, l) = item.GetFractionOfDay();
			if (item.Indent == 0 && b > end)
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
	public static IList<IControl> GetIndentationItems(IList<IControl> list, int beginIndex, int count, int indent)
	{
		var result = new List<IControl>();
		for (var i = beginIndex; i < beginIndex + count; i++)
		{
			var item = list[i].GetFirstLogicalDescendant<IAppointmentControl>();
			if (item.Indent == indent)
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
	public static double GetEnd(IList<IControl> list, int beginIndex, int count)
	{
		var item = list[beginIndex].GetFirstLogicalDescendant<IAppointmentControl>();
		var (begin, length) = item.GetFractionOfDay();
		var end = begin + length;

		for (var i = beginIndex + 1; i < beginIndex + count; i++)
		{
			item = list[i].GetFirstLogicalDescendant<IAppointmentControl>();
			var (b, l) = item.GetFractionOfDay();
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
	public static int GetIndentationCount(IList<IControl> list, int beginIndex, int count)
	{
		var result = 0;
		for (var i = beginIndex; i < beginIndex + count; i++)
		{
			var item = list[i].GetFirstLogicalDescendant<IAppointmentControl>();
			if (item.Indent > result)
				result = item.Indent;
		}

		return result + 1;
	}
}