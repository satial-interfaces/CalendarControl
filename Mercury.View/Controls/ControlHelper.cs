using System.Collections.Generic;
using Avalonia.Controls;

namespace Mercury.View;

public static class ControlHelper
{
    public static void AddControlsToRows(Grid grid, List<IControl?> controls, RowDefinitions rowDefinitions)
    {
        grid.RowDefinitions = rowDefinitions;
        grid.Children.Clear();
        foreach (var control in controls)
        {
            if (control != null)
                grid.Children.Add(control);
        }
        for (var i = 0; i < controls.Count; i++)
        {
            if (controls[i] != null)
                Grid.SetRow(controls[i] as Control, i);
        }
    }
}