using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Media;

namespace SatialInterfaces.Controls;

/// <summary>
/// This class represents an appointment and extends Border by adding an index
/// </summary>
[PseudoClasses(":pressed", ":selected")]
public class AppointmentControl : Border, ISelectable
{
	/// <summary>Begin property</summary>
	public static readonly DirectProperty<AppointmentControl, DateTime> BeginProperty = AvaloniaProperty.RegisterDirect<AppointmentControl, DateTime>(nameof(Begin), o => o.Begin, (o, v) => o.Begin = v);
	/// <summary>End property</summary>
	public static readonly DirectProperty<AppointmentControl, DateTime> EndProperty = AvaloniaProperty.RegisterDirect<AppointmentControl, DateTime>(nameof(End), o => o.End, (o, v) => o.End = v);
	/// <summary>Text property</summary>
	public static readonly DirectProperty<AppointmentControl, string> TextProperty = AvaloniaProperty.RegisterDirect<AppointmentControl, string>(nameof(Text), o => o.Text, (o, v) => o.Text = v);
	/// <summary>Color property</summary>
	public static readonly DirectProperty<AppointmentControl, Color> ColorProperty = AvaloniaProperty.RegisterDirect<AppointmentControl, Color>(nameof(Color), o => o.Color, (o, v) => o.Color = v);
	/// <inheritdoc />
	public bool IsSelected { get => isSelected; set { isSelected = value; PseudoClasses.Set(":selected", isSelected); } }
	/// <summary>Begin property</summary>
	public DateTime Begin { get => begin; set => SetAndRaise(BeginProperty, ref begin, value); }
	/// <summary>End property</summary>
	public DateTime End { get => end; set => SetAndRaise(EndProperty, ref end, value); }
	/// <summary>Text property</summary>
	public string Text { get => text; set => SetAndRaise(TextProperty, ref text, value); }
	/// <summary>Color property</summary>
	public Color Color { get => color; set => SetAndRaise(ColorProperty, ref color, value); }
	/// <summary>Index</summary>
	public int Index { get; set; }
	/// <summary>Indent</summary>
	public int Indent { get; set; }

	/// <summary>
	/// Gets the begin and length as a fraction of the day
	/// </summary>
	/// <returns>Begin and length as a fraction of the day</returns>
	public (double Begin, double Length) GetFractionOfDay()
	{
		var beginTimeSpan = Begin.TimeOfDay;
		var endTimeSpan = End.Subtract(Begin.Date);
		var b = beginTimeSpan.TotalHours / 24.0d;
		var e = endTimeSpan.TotalHours / 24.0d;
		if (e > 1.0d)
			e = 1.0d;
		var length = e - b;
		return (b, length);
	}

	/// <summary>
	/// Checks if this instance is in the given day
	/// </summary>
	/// <param name="dateTime">date/time for the given day</param>
	/// <returns>True if it is and false otherwise</returns>
	public bool IsInDay(DateTime dateTime)
	{
		var beginDay = dateTime.Date;
		var endDay = beginDay.AddDays(1).AddTicks(-1);
		return Begin >= beginDay && Begin < endDay;
	}

	/// <summary>
	/// Checks if this instance has overlap with another instance
	/// </summary>
	/// <param name="other">Other instance to check with</param>
	/// <returns>True if it has and false otherwise</returns>
	public bool HasOverlap(AppointmentControl other) => (other.Begin >= Begin && other.Begin < End) || (Begin >= other.Begin && Begin < other.End);

	/// <summary>
	/// Check if a given item is in the given week.
	/// </summary>
	/// <param name="beginWeek">Begin of the week</param>
	/// <returns>True if it is and false otherwise</returns>
	public bool IsInCurrentWeek(DateTime beginWeek) => Begin >= beginWeek;

	/// <summary>Is selected</summary>
	bool isSelected;
	/// <summary>Begin</summary>
	DateTime begin;
	/// <summary>End</summary>
	DateTime end;
	/// <summary>Text</summary>
	string text = "";
	/// <summary>Color</summary>
	Color color;
}