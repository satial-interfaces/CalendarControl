using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Mercury.View;

namespace SampleApp;

public class ColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Status s)
        {
            if (s == Status.Information)
                return Colors.Green;
            if (s == Status.Warning)
                return Colors.Orange;
            if (s == Status.Error)
                return Colors.Red;
        }

        return Colors.Transparent;
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

class CalendarControlItem
{
    public DateTime Begin { get; set; }
    public DateTime End { get; set; }
    public string Text { get; set; } = "";
    public Status Status { get; set; }
}

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
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
        var calendarControl = this.FindControl<CalendarControl>("CalendarControl");
        calendarControl.CurrentWeek = calendarControl.CurrentWeek.AddDays(-7);
    }
    protected void ThisWeekButtonClick(object? sender, RoutedEventArgs e)
    {
        var calendarControl = this.FindControl<CalendarControl>("CalendarControl");
        calendarControl.CurrentWeek = DateTime.Now;
    }
    protected void NextButtonClick(object? sender, RoutedEventArgs e)
    {
        var calendarControl = this.FindControl<CalendarControl>("CalendarControl");
        calendarControl.CurrentWeek = calendarControl.CurrentWeek.AddDays(7);
    }

    void RandomCalendar()
    {
        var calendarControl = this.FindControl<CalendarControl>("CalendarControl");
        var textBox = this.FindControl<TextBox>("TextBox");
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

            var item = new CalendarControlItem()
            {
                Begin = beginOfWeek.AddHours(begin),
                End = beginOfWeek.AddHours(begin + length),
                Text = $"App{i}",
                Status = GetRandom()
            };
            list.Add(item);
        }
        calendarControl.Items = list;
        var newList = new List<string>();
        for (var i = 0; i < list.Count; i++)
        {
            newList.Add(list[i].Text + ": " + list[i].Begin.ToShortTimeString() + " - " + list[i].End.ToShortTimeString());
        }
        textBox.Text = StringsToString(newList, Environment.NewLine);
    }

	static string StringsToString(IEnumerable<string?> list, string terminator)
	{
		var result = new StringBuilder();
		using var enumerator = list.GetEnumerator();
		var i = 0;
		while (enumerator.MoveNext())
		{
			var value = enumerator.Current;
			if (value == null) continue;
			if (i > 0)
				result.Append(terminator);
			result.Append(value);
			i++;
		}

		return result.ToString();
	}

	/// <summary>
	///     Gets a random int.
	/// </summary>
	/// <param name="minVal">Minimum value.</param>
	/// <param name="maxVal">Maximum value (inclusive).</param>
	/// <returns>The random int.</returns>
	static int GetRandom(int minVal, int maxVal)
	{
		var result = random.Next(minVal, maxVal + 1);
		return result;
	}

    static Status GetRandom()
    {
        var i = GetRandom((int)Status.None, (int)Status.Error);
        return (Status)i;
    }

    static DateTime GetBeginWeek(DateTime dateTime, DayOfWeek firstDayOfWeek)
    {
        var dayOfWeek = (int)dateTime.DayOfWeek;
        var diff = (int)firstDayOfWeek - dayOfWeek;
        var begin = diff <= 0 ? dateTime.AddDays(diff) : dateTime.AddDays(diff - 7);
        return begin.Date;
    }

    static readonly Random random = new();
}
