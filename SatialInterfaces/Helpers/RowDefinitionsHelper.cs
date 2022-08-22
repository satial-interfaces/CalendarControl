using System.Collections.Generic;
using Avalonia.Controls;

namespace SatialInterfaces.Helpers;

/// <summary>This class contains RowDefinitions helpers method.</summary>
public static class RowDefinitionsHelper
{
	/// <summary>
	/// Adds an empty row for the tail (if applicable)
	/// </summary>
	/// <param name="rowDefinitions">List of row definitions to add to</param>
	/// <param name="controls">List of controls to add to</param>
	/// <param name="previousEnd">End of the previous appointment (as a fraction of the day)</param>
	public static void AddEmptyRowTail(RowDefinitions rowDefinitions, ICollection<IControl?> controls, double previousEnd)
	{
		if (!double.IsNaN(previousEnd) && previousEnd >= 1.0d)
			return;

		controls.Add(null);
		rowDefinitions.Add(new RowDefinition(1.0d - (!double.IsNaN(previousEnd) ? previousEnd : 0.0d), GridUnitType.Star));
	}

	/// <summary>
	/// Adds an empty row (if applicable)
	/// </summary>
	/// <param name="rowDefinitions">List of row definitions to add to</param>
	/// <param name="controls">List of controls to add to</param>
	/// <param name="previousEnd">End of the previous appointment (as a fraction of the day)</param>
	/// <param name="begin">Begin of the current appointment (as a fraction of the day)</param>
	public static void AddEmptyRow(RowDefinitions rowDefinitions, ICollection<IControl?> controls, double previousEnd, double begin)
	{
		var emptyLength = begin - (!double.IsNaN(previousEnd) ? previousEnd : 0.0d);
		if (emptyLength <= 0.0d)
			return;

		controls.Add(null);
		rowDefinitions.Add(new RowDefinition(emptyLength, GridUnitType.Star));
	}
}
