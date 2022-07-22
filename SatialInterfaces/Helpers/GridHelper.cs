using System.Collections.Generic;
using Avalonia.Controls;

namespace SatialInterfaces.Helpers;

/// <summary>
/// This class contains Grid helper methods
/// </summary>
public static class GridHelper
{
	/// <summary>
	/// Adds controls to the given grid's rows
	/// </summary>
	/// <param name="grid">Grid to add to</param>
	/// <param name="rowDefinitions">List of row definitions</param>
	/// <param name="controls">Controls to adds</param>
	public static void AddRows(Grid grid, RowDefinitions rowDefinitions, IReadOnlyList<IControl?> controls)
	{
		grid.RowDefinitions = rowDefinitions;
		grid.Children.Clear();

		List<IControl> controlsToAdd = new();
		for (var i = 0; i < controls.Count; i++)
		{
			if (controls[i] is not Control controlToAdd) continue;
			Grid.SetRow(controlToAdd, i);
			controlsToAdd.Add(controlToAdd);
		}

		grid.Children.AddRange(controlsToAdd);
	}
}