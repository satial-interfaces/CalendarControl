using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using CalendarControl.Controls;

namespace SampleApp;

public class ColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Status s) return Colors.Transparent;
        return s switch
        {
            Status.Information => Colors.Green,
            Status.Warning => Colors.Orange,
            Status.Error => Colors.Red,
            _ => Colors.Transparent,
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
public enum Status
{
    None,
    Information,
    Warning,
    Error
}

internal class CalendarControlItem
{
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
    public string Text { get; set; } = "";
    public Status Status { get; set; }
}

public partial class MainWindow : Window
{
#pragma warning disable CS8618
    public MainWindow()
#pragma warning restore CS8618
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        calendarControl = this.FindControl<CalendarControl.Controls.CalendarControl>("CalendarControl");
        RandomCalendar();
    }

    List<CalendarControlItem> list = new();

    protected void CalendarControlSelectionChanged(object? sender, CalendarSelectionChangedEventArgs e)
	{
        Title = e.SelectedIndex.ToString();
        var info = this.FindControl<TextBlock>("Info");
        if (e.SelectedIndex >= 0)
        {
            var item = list[e.SelectedIndex];
            info.Text = item.Text + ": " + item.Begin.ToShortTimeString() + " - " + item.End.ToShortTimeString();
        }
        else
        {
            info.Text = "";
        }
    }

    protected void RandomButtonClick(object? sender, RoutedEventArgs e)
    {
        RandomCalendar();
    }
    protected void PreviousButtonClick(object? sender, RoutedEventArgs e)
    {
        calendarControl.CurrentWeek = calendarControl.CurrentWeek.AddDays(-7);
    }
    protected void ThisWeekButtonClick(object? sender, RoutedEventArgs e)
    {
        calendarControl.CurrentWeek = DateTime.Now;
    }
    protected void NextButtonClick(object? sender, RoutedEventArgs e)
    {
        calendarControl.CurrentWeek = calendarControl.CurrentWeek.AddDays(7);
    }

    void RandomCalendar()
    {
        calendarControl.CurrentWeek = DateTime.Now;
        var beginOfWeek = GetBeginWeek(calendarControl.CurrentWeek, calendarControl.FirstDayOfWeek);

        list = new List<CalendarControlItem>();
        const int weeks = 1;
        const int heads = weeks / 2;
        const int tails = weeks - heads;
        for (var i = 0; i < 24 * weeks; i++)
        {
            var begin = GetRandom(-168 * heads * 4, 168 * tails * 4) / 4.0d;
            var length = GetRandom(1, 8);

            var item = new CalendarControlItem
            {
                Begin = beginOfWeek.AddHours(begin),
                End = beginOfWeek.AddHours(begin + length),
                Text = $"Appointment {i}",
                Status = GetRandom()
            };
            list.Add(item);
        }
        calendarControl.Items = list;
    }

    static int GetRandom(int minVal, int maxVal) => random.Next(minVal, maxVal + 1);

    static Status GetRandom() => (Status)GetRandom((int)Status.None, (int)Status.Error);

    static DateTime GetBeginWeek(DateTime dateTime, DayOfWeek firstDayOfWeek)
    {
        var dayOfWeek = (int)dateTime.DayOfWeek;
        var diff = (int)firstDayOfWeek - dayOfWeek;
        var begin = diff <= 0 ? dateTime.AddDays(diff) : dateTime.AddDays(diff - 7);
        return begin.Date;
    }

    CalendarControl.Controls.CalendarControl calendarControl;
    static readonly Random random = new();
}
