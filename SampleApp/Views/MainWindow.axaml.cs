using System;
using System.Globalization;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using SatialInterfaces.Controls;

namespace SampleApp.Views;

public class ColorConverter : IValueConverter
{
	public static readonly ColorConverter Instance = new();

	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not Status s) return Colors.Transparent;
		return s switch
		{
			Status.Information => Colors.Green,
			Status.Warning => Colors.Orange,
			Status.Error => Colors.Red,
			_ => Colors.Transparent
		};
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}

public enum Status
{
	None,
	Information,
	Warning,
	Error
}

public class AppointmentViewModel
{
	public DateTime Begin { get; set; }
	public DateTime End { get; set; }
	public string Text { get; set; } = "";
	public Status Status { get; set; }
}

public class MainWindow : Window
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

	void InitializeComponent()
	{
		AvaloniaXamlLoader.Load(this);
		calendarControl = this.FindControl<CalendarControl>("CalendarControl");
		Dispatcher.UIThread.Post(() => calendarControl.Focus());
		RandomCalendar();
	}

#pragma warning disable RCS1213
	void CalendarControlSelectionChanged(object? sender, CalendarSelectionChangedEventArgs e)
#pragma warning restore RCS1213
	{
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

#pragma warning disable RCS1213
	void RandomButtonClick(object? sender, RoutedEventArgs e) => RandomCalendar();
#pragma warning restore RCS1213

#pragma warning disable RCS1213
	void PreviousButtonClick(object? sender, RoutedEventArgs e) => calendarControl.CurrentWeek = calendarControl.CurrentWeek.AddDays(-7);
#pragma warning restore RCS1213

#pragma warning disable RCS1213
	void ThisWeekButtonClick(object? sender, RoutedEventArgs e) => calendarControl.CurrentWeek = DateTime.Now;
#pragma warning restore RCS1213

#pragma warning disable RCS1213
	void NextButtonClick(object? sender, RoutedEventArgs e) => calendarControl.CurrentWeek = calendarControl.CurrentWeek.AddDays(7);
#pragma warning restore RCS1213

#pragma warning disable RCS1213
	void NewButtonClick(object? sender, RoutedEventArgs e)
#pragma warning restore RCS1213
	{
		var beginOfWeek = GetBeginWeek(calendarControl.CurrentWeek, calendarControl.FirstDayOfWeek).AddDays(7 + 3);
		var begin = GetRandom(0, 24 * 4) / 4.0d;
		var length = GetRandom(1, 8);

		var item = new AppointmentViewModel
		{
			Begin = beginOfWeek.AddHours(begin),
			End = beginOfWeek.AddHours(begin + length),
			Text = $"New Appointment {list.Count}",
			Status = GetRandom()
		};
		list.Add(item);
		// list = new List<AppointmentViewModel>(list);
		// calendarControl.Items = list;
		calendarControl.ScrollIntoView(item);
	}

	void RandomCalendar()
	{
		calendarControl.CurrentWeek = DateTime.Now;
		var beginOfWeek = GetBeginWeek(calendarControl.CurrentWeek, calendarControl.FirstDayOfWeek);

		list = new AvaloniaList<AppointmentViewModel>();
		const int weeks = 1;
		const int heads = weeks / 2;
		const int tails = weeks - heads;
		for (var i = 0; i < 24 * weeks; i++)
		{
			var begin = GetRandom(-168 * heads * 4, 168 * tails * 4) / 4.0d;
			var length = GetRandom(1, 8);

			var item = new AppointmentViewModel
			{
				Begin = beginOfWeek.AddHours(begin),
				End = beginOfWeek.AddHours(begin + length),
				Text = $"Appointment {i}",
				Status = GetRandom()
			};
			list.Add(item);
		}

		calendarControl.Items = list;
		calendarControl.SelectedIndex = 0;
	}

	static int GetRandom(int minVal, int maxVal) => Random.Next(minVal, maxVal + 1);

	static Status GetRandom() => (Status)GetRandom((int)Status.None, (int)Status.Error);

	static DateTime GetBeginWeek(DateTime dateTime, DayOfWeek firstDayOfWeek)
	{
		var dayOfWeek = (int)dateTime.DayOfWeek;
		var diff = (int)firstDayOfWeek - dayOfWeek;
		var begin = diff <= 0 ? dateTime.AddDays(diff) : dateTime.AddDays(diff - 7);
		return begin.Date;
	}
	static readonly Random Random = new();
	CalendarControl calendarControl;
	AvaloniaList<AppointmentViewModel> list = new();
}