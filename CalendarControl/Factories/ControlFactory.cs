using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Media;
using CalendarControl.Controls;
using CalendarControl.Helpers;

namespace CalendarControl.Factories{

/// <summary>
/// This class contains control factory methods.
/// </summary>
public static class ControlFactory
{
    /// <summary>
    /// Creates a column
    /// </summary>
    /// <returns>Created column</returns>
   public static Grid CreateColumn() => new();

    /// <summary>
    /// Creates a workday/weekend control
    /// </summary>
    /// <returns>Created control</returns>
    public static Control CreateDayState(DayOfWeek dayOfWeek)
    {
        var dayState = DateTimeHelper.GetDayState(CultureInfo.CurrentCulture, dayOfWeek);
        if (dayState == DateTimeHelper.DayState.WorkDay) return new Border();
        if (dayState == DateTimeHelper.DayState.WorkdayMorning)
        {
            var grid = new Grid
            {
                RowDefinitions = new RowDefinitions
                {
                    new RowDefinition(1.0d, GridUnitType.Star),
                    new RowDefinition(1.0d, GridUnitType.Star)
                }
            };
            var border = new Border();
            var weekendControl = new WeekendControl();
            grid.Children.Add(border);
            grid.Children.Add(weekendControl);
            Grid.SetRow(border, 0);
            Grid.SetRow(weekendControl, 1);
            return grid;
        }
        return new WeekendControl();
    }

    /// <summary>
    /// Creates a background cell
    /// </summary>
    /// <returns>Created control</returns>
    public static Border CreateHourCell()
    {
        return new HourCell();
    }

    /// <summary>
    /// Creates an appointment
    /// </summary>
    /// <param name="begin">Begin of the appointment</param>
    /// <param name="text">Text to add to appointment</param>
    /// <param name="color">Color to add to appointment</param>
    /// <param name="index">Index to set to appointment</param>
    /// <returns>Created appointment</returns>
    public static Border CreateAppointment(DateTime begin, string text, Color color, int index)
    {
        var border = new AppointmentControl { Index = index };
        if (color != Colors.Transparent)
        {
            border.Background = new SolidColorBrush(color);
        }

        var textBlock = new TextBlock { Text = begin.ToShortTimeString() + Environment.NewLine + text };
        var grid = new Grid();
        grid.Children.Add(new Border());
        grid.Children.Add(textBlock);

        border.Child = grid;
        return border;
    }

    /// <summary>
    /// Creates a grid with a given column count
    /// </summary>
    /// <param name="columnCount">Column count to assign</param>
    /// <returns>The grid</returns>
    public static Grid CreateGrid(int columnCount)
    {
        var grid = new Grid();
        var columnDefinitions = new ColumnDefinitions();
        for (var i = 0; i < columnCount; i++)
        {
            columnDefinitions.Add(new ColumnDefinition(1.0d, GridUnitType.Star));
            grid.Children.Add(new Grid());
        }
        grid.ColumnDefinitions = columnDefinitions;
        for (var i = 0; i < columnCount; i++)
            Grid.SetColumn(grid.Children[i] as Control, i);
        return grid;
    }
}
}
