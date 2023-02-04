using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace SatialInterfaces.Helpers;

/// <summary>This class contains control helper methods.</summary>
public static class ControlHelper
{
	/// <summary>
	/// Adds controls to the rows of a grid
	/// </summary>
	/// <param name="grid">Grid to add to</param>
	/// <param name="controls">Controls to add</param>
	/// <param name="rowDefinitions">Row definitions</param>
	public static void AddControlsToRows(Grid grid, List<Control?> controls, RowDefinitions rowDefinitions)
	{
		grid.RowDefinitions = rowDefinitions;
		grid.Children.Clear();
		foreach (var control in controls.Where(x => x != null))
		{
			if (control == null) continue;
			grid.Children.Add(control);
		}
		for (var i = 0; i < controls.Count; i++)
		{
			if (controls[i] is not {} c)
				continue;

			Grid.SetRow(c, i);
		}
	}

	/// <summary>
	/// Check if the first logical descendant (including itself) matches the given type
	/// </summary>
	/// <param name="obj">Object to act on</param>
	/// <typeparam name="T">Type to search for</typeparam>
	/// <returns>True if it does and false otherwise</returns>
	public static bool HasFirstLogicalDescendant<T>(this ILogical obj) where T : ILogical
	{
		if (obj is T) return true;
		return obj.GetLogicalDescendants().OfType<T>().FirstOrDefault() != null;
	}

	/// <summary>
	/// Gets the first logical descendant (including itself) that matches the given type
	/// </summary>
	/// <param name="obj">Object to act on</param>
	/// <typeparam name="T">Type to search for</typeparam>
	/// <returns>The first match or null otherwise</returns>
	public static T GetFirstLogicalDescendant<T>(this ILogical obj) where T : ILogical
	{
		if (obj is T t) return t;
		return obj.GetLogicalDescendants().OfType<T>().FirstOrDefault()!;
	}
}