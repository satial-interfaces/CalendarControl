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
	/// <summary>The index property</summary>
	public static readonly DirectProperty<AppointmentControl, int> IndexProperty = AvaloniaProperty.RegisterDirect<AppointmentControl, int>(nameof(Index), o => o.Index, (o, v) => o.Index = v);
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
	public int Index { get => index; set => SetAndRaise(IndexProperty, ref index, value); }
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
	/// <summary>Index</summary>
	int index;
}