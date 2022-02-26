using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;

namespace SatialInterfaces.Helpers;

/// <summary>
/// This class contains control helper methods
/// </summary>
public static class ControlHelper
{
    /// <summary>
    /// Adds controls to the rows of a grid
    /// </summary>
    /// <param name="grid">Grid to add to</param>
    /// <param name="controls">Controls to add</param>
    /// <param name="rowDefinitions">Row definitions</param>
    public static void AddControlsToRows(Grid grid, List<IControl?> controls, RowDefinitions rowDefinitions)
    {
        grid.RowDefinitions = rowDefinitions;
        grid.Children.Clear();
        foreach (var control in controls.Where(x => x != null))
            grid.Children.Add(control);
        for (var i = 0; i < controls.Count; i++)
            if (controls[i] != null)
                Grid.SetRow(controls[i] as Control, i);
    }
}