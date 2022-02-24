using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using CalendarControl.Controls;
using CalendarControl.Converters;
using CalendarControl.Helpers;

namespace CalendarControl.Factories;

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
        if (dayState != DateTimeHelper.DayState.WorkdayMorning) return new WeekendControl();
        var grid = new Grid
        {
            RowDefinitions = new RowDefinitions
            {
                new(1.0d, GridUnitType.Star),
                new(1.0d, GridUnitType.Star)
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

    /// <summary>
    /// Creates a background cell
    /// </summary>
    /// <returns>Created control</returns>
    public static Border CreateHourCell()
    {
        return new HourCell();
    }

    /// <summary>
    /// Creates an appointment control for the given item
    /// </summary>
    /// <param name="item">Appointment item to create control for</param>
    /// <returns>Created appointment</returns>
    public static Border CreateAppointment(AppointmentItem item)
    {
        var border = new AppointmentControl { Index = item.Index };
        border.DataContext = item;
        if (item.Color != Colors.Transparent)
        {
            border[!Border.BackgroundProperty] = new Binding("Color")
            {
                Converter = OneWayConverter<Color, IBrush>.GetInstance((v, _, _) => new SolidColorBrush(v))
            };
        }

        var textBlock = new TextBlock();
        textBlock[!TextBlock.TextProperty] = new Binding("Text")
        {
            Converter = OneWayConverter<string, string>.GetInstance((v, p, _) =>
            {
                if (p is not AppointmentItem i) return "";
                return i.Begin.ToShortTimeString() + Environment.NewLine + v;
            }),
            ConverterParameter = item
        };
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
