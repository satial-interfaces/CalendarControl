using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using SatialInterfaces.Controls;

namespace SatialInterfaces.Helpers;

internal static class AppointmentControlListHelper
{
	/// <summary>
	/// Applies the indentation of the appointment list (based on avoiding overlap)
	/// </summary>
	/// <param name="list">Appointment list to process</param>
	public static void ApplyIndentation(IEnumerable<Control> list)
	{
		var listOfLists = new List<List<Control>>();

		foreach (var item in list)
		{
			var index = GetIndent(listOfLists, item);
			List<Control> indentList;
			if (index >= 0)
			{
				indentList = listOfLists[index];
			}
			else
			{
				indentList = new List<Control>();
				listOfLists.Add(indentList);
			}

			indentList.Add(item);
		}

		for (var i = 0; i < listOfLists.Count; i++)
		{
			foreach (var item in listOfLists[i])
			{
				item.GetFirstLogicalDescendant<IAppointmentControl>().Indent = i;
			}
		}
	}

	/// <summary>
	/// Gets the index of the list in which the given item fits
	/// </summary>
	/// <param name="listOfLists">List with indentations</param>
	/// <param name="item">Item to check</param>
	/// <returns>The index or -1 otherwise</returns>
	static int GetIndent(IReadOnlyList<List<Control>> listOfLists, ILogical item)
	{
		for (var i = 0; i < listOfLists.Count; i++)
		{
			if (FitsIn(listOfLists[i], item))
				return i;
		}

		return -1;
	}

	/// <summary>
	/// Checks if the given item fits in the list (has no overlap)
	/// </summary>
	/// <param name="list">List to check in</param>
	/// <param name="item">Item to check</param>
	/// <returns>True if it fits and false otherwise</returns>
	static bool FitsIn(IEnumerable<Control> list, ILogical item)
	{
		var item2 = item.GetFirstLogicalDescendant<IAppointmentControl>();
		return list.All(x => !x.GetFirstLogicalDescendant<IAppointmentControl>().HasOverlap(item2));
	}
}