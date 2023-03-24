using Avalonia.Interactivity;

namespace SatialInterfaces.Controls.Calendar;

/// <summary>Expansion of the RoutedEventArgs class by providing a selected index;.</summary>
public class CalendarSelectionChangedEventArgs : RoutedEventArgs
{
	/// <summary>
	/// Initializes a new instance of the CalendarSelectionChangedEventArgs class, using the supplied routed event
	/// identifier.
	/// </summary>
	/// <param name="routedEvent">The routed event identifier for this instance of the CalendarSelectionChangedEventArgs class.</param>
	public CalendarSelectionChangedEventArgs(RoutedEvent routedEvent) : base(routedEvent)
	{
	}

	/// <summary>
	/// Property representing the selected index. -1 means not selected.
	/// </summary>
	/// <value>The selected index (zero-based) or -1 if deselected.</value>
	public int SelectedIndex { get; set; }

	/// <summary>
	/// Property representing the selected object. Null means not selected.
	/// </summary>
	/// <value>The selected object or null if deselected.</value>
	public object? SelectedItem { get; set; }
}