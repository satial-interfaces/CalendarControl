using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace SatialInterfaces.Controls.Calendar;

/// <summary>This interface represents an appointment.</summary>
public interface IAppointmentControl : ILogical, IDataContextProvider, ISelectable
{
	/// <summary>Begin property</summary>
	DateTime Begin { get; set; }
	/// <summary>End property</summary>
	DateTime End { get; set; }
	/// <summary>Text property</summary>
	string Text { get; set; }
	/// <summary>Color property</summary>
	Color Color { get; set; }
	/// <summary>Index property</summary>
	int Index { get; set; }
	/// <summary>Indent property</summary>
	int Indent { get; set; }
	/// <summary>
	/// Gets the begin and length as a fraction of the day
	/// </summary>
	/// <returns>Begin and length as a fraction of the day</returns>
	(double Begin, double Length) GetFractionOfDay();
	/// <summary>
	/// Checks if this instance has overlap with another instance
	/// </summary>
	/// <param name="other">Other instance to check with</param>
	/// <returns>True if it has and false otherwise</returns>
	bool HasOverlap(IAppointmentControl other);
	/// <summary>
	/// Check if a given item is in the given week.
	/// </summary>
	/// <param name="beginWeek">Begin of the week</param>
	/// <returns>True if it is and false otherwise</returns>
	bool IsInWeek(DateTime beginWeek);
	/// <summary>
	/// Checks if this instance is in the given day
	/// </summary>
	/// <param name="dateTime">date/time for the given day</param>
	/// <returns>True if it is and false otherwise</returns>
	bool IsInDay(DateTime dateTime);
	/// <summary>
	/// Checks if this instance is valid
	/// </summary>
	/// <returns>True if it is and false otherwise</returns>
	bool IsValid();
}

/// <summary>This class represents an appointment and extends Border.</summary>
[PseudoClasses(":pressed", ":selected")]
public class AppointmentControl : Border, IAppointmentControl
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
	/// <inheritdoc />
	public DateTime Begin { get => begin; set => SetAndRaise(BeginProperty, ref begin, value); }
	/// <inheritdoc />
	public DateTime End { get => end; set => SetAndRaise(EndProperty, ref end, value); }
	/// <inheritdoc />
	public string Text { get => text; set => SetAndRaise(TextProperty, ref text, value); }
	/// <inheritdoc />
	public Color Color { get => color; set => SetAndRaise(ColorProperty, ref color, value); }
	/// <inheritdoc />
	public int Index { get; set; }
	/// <inheritdoc />
	public int Indent { get; set; }

	/// <inheritdoc />
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

	/// <inheritdoc />
	public bool IsInDay(DateTime dateTime)
	{
		var beginDay = dateTime.Date;
		var endDay = beginDay.AddDays(1).AddTicks(-1);
		return Begin >= beginDay && Begin < endDay;
	}

	/// <inheritdoc />
	public bool IsValid()
	{
		if (Begin == EmptyDateTime || End == EmptyDateTime)
			return false;
		return End > Begin;
	}

	/// <inheritdoc />
	public bool HasOverlap(IAppointmentControl other) => (other.Begin >= Begin && other.Begin < End) || (Begin >= other.Begin && Begin < other.End);

	/// <inheritdoc />
	public bool IsInWeek(DateTime beginWeek) => Begin >= beginWeek && Begin < beginWeek.AddDays(7);

	/// <summary>An empty date/time</summary>
	static readonly DateTime EmptyDateTime = new();
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