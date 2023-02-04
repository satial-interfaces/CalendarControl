using System.Collections.Generic;
using Avalonia.Controls;

namespace SatialInterfaces.Helpers;

/// <summary>This class contains Grid helper methods.</summary>
public static class GridHelper
{
	/// <summary>
	/// Adds controls to the given grid's rows
	/// </summary>
	/// <param name="grid">Grid to add to</param>
	/// <param name="rowDefinitions">List of row definitions</param>
	/// <param name="controls">Controls to adds</param>
	public static void AddRows(Grid grid, RowDefinitions rowDefinitions, IReadOnlyList<Control?> controls)
	{
		grid.RowDefinitions = rowDefinitions;
		grid.Children.Clear();

		List<Control> controlsToAdd = new();
		for (var i = 0; i < controls.Count; i++)
		{
			if (controls[i] is not { } control) continue;
			Grid.SetRow(control, i);
			controlsToAdd.Add(control);
		}

		grid.Children.AddRange(controlsToAdd);
	}
}