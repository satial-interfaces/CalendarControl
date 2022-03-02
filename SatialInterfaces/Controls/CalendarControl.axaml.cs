using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Avalonia.Threading;
using SatialInterfaces.Factories;
using SatialInterfaces.Helpers;

namespace SatialInterfaces.Controls;

/// <summary>
/// This class represents a calendar control (week view)
/// </summary>
public class CalendarControl : ContentControl, IStyleable
{
	/// <summary>Allow delete property</summary>
	public static readonly DirectProperty<CalendarControl, bool> AllowDeleteProperty = AvaloniaProperty.RegisterDirect<CalendarControl, bool>(nameof(AllowDelete), o => o.AllowDelete, (o, v) => o.AllowDelete = v);
	/// <summary>Margin around the appointment group property</summary>
	public static readonly StyledProperty<Thickness> AppointmentGroupMarginProperty = AvaloniaProperty.Register<CalendarControl, Thickness>(nameof(AppointmentGroupMargin));
	/// <summary>First day of the week property</summary>
	public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty = AvaloniaProperty.Register<CalendarControl, DayOfWeek>(nameof(FirstDayOfWeek), DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek);
	/// <summary>Items property</summary>
	public static readonly DirectProperty<CalendarControl, IEnumerable> ItemsProperty = AvaloniaProperty.RegisterDirect<CalendarControl, IEnumerable>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
	/// <summary>Current week property</summary>
	public static readonly DirectProperty<CalendarControl, DateTime> CurrentWeekProperty = AvaloniaProperty.RegisterDirect<CalendarControl, DateTime>(nameof(CurrentWeek), o => o.CurrentWeek, (o, v) => o.CurrentWeek = v);
	/// <summary>The begin of the day property</summary>
	public static readonly DirectProperty<CalendarControl, TimeSpan> BeginOfTheDayProperty = AvaloniaProperty.RegisterDirect<CalendarControl, TimeSpan>(nameof(BeginOfTheDay), o => o.BeginOfTheDay, (o, v) => o.BeginOfTheDay = v);
	/// <summary>The end of the day property</summary>
	public static readonly DirectProperty<CalendarControl, TimeSpan> EndOfTheDayProperty = AvaloniaProperty.RegisterDirect<CalendarControl, TimeSpan>(nameof(EndOfTheDay), o => o.EndOfTheDay, (o, v) => o.EndOfTheDay = v);
	/// <summary>Item template</summary>
	public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<CalendarControl, IDataTemplate?>(nameof(ItemTemplate));
	/// <summary>The selected index property</summary>
	public static readonly StyledProperty<int> SelectedIndexProperty = AvaloniaProperty.Register<CalendarControl, int>(nameof(SelectedIndex), -1);
	/// <summary>The selected item property</summary>
	public static readonly StyledProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<CalendarControl, object?>(nameof(SelectedItem));
	/// <summary>The selection changed event</summary>
	public static readonly RoutedEvent<CalendarSelectionChangedEventArgs> SelectionChangedEvent = RoutedEvent.Register<CalendarControl, CalendarSelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

	/// <summary>
	/// Initializes static members of the <see cref="CalendarControl" /> class.
	/// </summary>
	static CalendarControl()
	{
		FocusableProperty.OverrideDefaultValue<CalendarControl>(true);

		ItemsProperty.Changed.AddClassHandler<CalendarControl>((x, e) => x.ItemsChanged(e));
		CurrentWeekProperty.Changed.AddClassHandler<CalendarControl>((x, e) => x.CurrentWeekChanged(e));
		SelectedIndexProperty.Changed.AddClassHandler<CalendarControl>((x, e) => x.SelectedIndexChanged(e));
		SelectedItemProperty.Changed.AddClassHandler<CalendarControl>((x, e) => x.SelectedItemChanged(e));
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CalendarControl" /> class.
	/// </summary>
	public CalendarControl()
	{
		AvaloniaXamlLoader.Load(this);

		var scrollViewer = this.FindControl<ScrollViewer>("ScrollViewer");
		scrollViewer.GetObservable(BoundsProperty).Subscribe(OnScrollViewerBoundsChanged);
		CreateWeek(CurrentWeek);
		UpdateItems(Items);
	}

	/// <inheritdoc />
	Type IStyleable.StyleKey => typeof(CalendarControl);
	/// <summary>Allow delete property</summary>
	public bool AllowDelete { get => allowDelete; set => SetAndRaise(AllowDeleteProperty, ref allowDelete, value); }
	/// <summary>Margin around the appointment group</summary>
	public Thickness AppointmentGroupMargin { get => GetValue(AppointmentGroupMarginProperty); set => SetValue(AppointmentGroupMarginProperty, value); }
	/// <summary>First day of the week property</summary>
	public DayOfWeek FirstDayOfWeek { get => GetValue(FirstDayOfWeekProperty); set => SetValue(FirstDayOfWeekProperty, value); }
	/// <summary>Items property</summary>
	public IEnumerable Items { get => items; set => SetAndRaise(ItemsProperty, ref items, value); }
	/// <summary>Current week property</summary>
	public DateTime CurrentWeek { get => currentWeek; set => SetAndRaise(CurrentWeekProperty, ref currentWeek, value); }
	/// <summary>Begin of the day property</summary>
	public TimeSpan BeginOfTheDay { get => beginOfTheDay; set => SetAndRaise(BeginOfTheDayProperty, ref beginOfTheDay, value); }
	/// <summary>End of the day property</summary>
	public TimeSpan EndOfTheDay { get => endOfTheDay; set => SetAndRaise(EndOfTheDayProperty, ref endOfTheDay, value); }
	/// <summary>Item template</summary>
	public IDataTemplate? ItemTemplate { get => GetValue(ItemTemplateProperty); set => SetValue(ItemTemplateProperty, value); }
	/// <summary>Selected index</summary>
	public int SelectedIndex { get => GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }
	/// <summary>Selected item</summary>
	public object? SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }
	/// <summary>Occurs when selection changed</summary>
	public event EventHandler<CalendarSelectionChangedEventArgs> SelectionChanged { add => AddHandler(SelectionChangedEvent, value); remove => RemoveHandler(SelectionChangedEvent, value); }

	/// <summary>
	/// Scrolls the specified item into view.
	/// </summary>
	/// <param name="index">The index of the item.</param>
	public void ScrollIntoView(int index)
	{
		if (index < 0 || index >= internalItems.Count) return;
		ScrollIntoView(internalItems, index);
	}

	/// <summary>
	/// Scrolls the specified item into view.
	/// </summary>
	/// <param name="item">The item</param>
	public void ScrollIntoView(object item)
	{
		var index = GetItemsAsList().IndexOf(item);
		if (index < 0 || index >= internalItems.Count) return;
		ScrollIntoView(internalItems, index);
	}

	/// <inheritdoc />
	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		leftButtonDown = e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;
		base.OnPointerPressed(e);
	}

	/// <inheritdoc />
	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		if (!Items.Any() || !leftButtonDown || e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
		{
			leftButtonDown = false;
			base.OnPointerReleased(e);
			return;
		}

		var control = e.Pointer.Captured as ILogical;
		var appointment = control?.FindLogicalAncestorOfType<AppointmentControl>();
		var index = appointment?.Index ?? -1;
		leftButtonDown = false;
		base.OnPointerReleased(e);
		var previousIndex = SelectedIndex;
		SelectedIndex = index;

		// Force retrigger
		if (index == previousIndex)
			RaiseSelectionChanged(index);
	}

	/// <inheritdoc />
	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.Key is Key.Up or Key.Left)
		{
			SelectNext(-1);
			e.Handled = true;
		}
		else if (e.Key is Key.Down or Key.Right)
		{
			SelectNext(1);
			e.Handled = true;
		}
		else if (e.Key is Key.Delete)
		{
			DeleteAppointment();
			e.Handled = true;
		}
		base.OnKeyDown(e);
	}

	/// <summary>
	/// Select next appointment
	/// </summary>
	/// <param name="step">Step to take</param>
	void SelectNext(int step)
	{
		var itemsGrid = this.FindControl<Grid>("ItemsGrid");
		var appointments = itemsGrid.GetLogicalDescendants().OfType<AppointmentControl>().ToList();
		if (appointments.Count == 0) return;
		var appointmentIndex = appointments.FindIndex(x => x.Index == SelectedIndex);
		appointmentIndex += step;
		if (appointmentIndex < 0)
			appointmentIndex = appointments.Count - 1;
		else if (appointmentIndex >= appointments.Count)
			appointmentIndex = 0;
		SelectedItem = appointments[appointmentIndex].DataContext;
	}

	/// <summary>
	/// Deletes the selected appointment
	/// </summary>
	void DeleteAppointment()
	{
		if (SelectedIndex < 0 || !AllowDelete) return;
		var list = GetItemsAsList();
		try
		{
			list.RemoveAt(SelectedIndex);
			skipItemsChanged = true;
			Items = new ArrayList();
			skipItemsChanged = false;
			Items = list;
		}
		catch (NotSupportedException)
		{
			// Too bad
		}
	}

	/// <summary>
	/// Gets the items as a list
	/// </summary>
	/// <returns></returns>
	IList GetItemsAsList()
	{
		if (Items is IList list) return list;
		return Items.ToList();
	}

	/// <summary>
	/// Scroll viewer bounds changed: adjust scrollable grid as well
	/// /// </summary>
	/// <param name="rect">Rectangle of the scroll viewer</param>
	void OnScrollViewerBoundsChanged(Rect rect)
	{
		if (rect.Height < 0) return;
		var scrollViewer = this.FindControl<ScrollViewer>("ScrollViewer");
		var scrollableGrid = this.FindControl<Grid>("ScrollableGrid");

		if (BeginOfTheDay.TotalDays >= 0.0d && EndOfTheDay.TotalDays < 24.0d && EndOfTheDay > BeginOfTheDay)
		{
			var length = (EndOfTheDay - BeginOfTheDay).TotalDays;
			var height = 1.0d / length * rect.Height;
			scrollableGrid.Height = height;

			if (scrollViewer.Offset.Y == 0.0d)
				Dispatcher.UIThread.Post(() => scrollViewer.Offset = new Vector(0.0d, BeginOfTheDay.TotalDays * height));
		}
		else
		{
			scrollableGrid.Height = rect.Height;
		}
	}

	/// <summary>
	/// Scrolls the specified item into view.
	/// </summary>
	/// <param name="list">List of the item</param>
	/// <param name="index">Index of the item</param>
	void ScrollIntoView(IList<IControl> list, int index)
	{
		var item = list[index].GetFirstLogicalDescendant<IAppointmentControl>();
		CurrentWeek = item.Begin;

		var scrollViewer = this.FindControl<ScrollViewer>("ScrollViewer");
		var scrollViewerRect = scrollViewer.Bounds;

		var begin = (item.Begin - item.Begin.Date).TotalDays;
		scrollViewer.Offset = new Vector(0.0d, begin * scrollViewerRect.Height);
	}

	/// <summary>
	/// Clear the selection bit for the other appointments.
	/// </summary>
	void ClearSelection()
	{
		var itemsGrid = this.FindControl<Grid>("ItemsGrid");
		foreach (var appointment in itemsGrid.GetLogicalDescendants().OfType<AppointmentControl>())
			appointment.IsSelected = false;
	}

	/// <summary>
	/// Items changed event
	/// </summary>
	/// <param name="e">Argument for the event</param>
	protected void ItemsChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (skipItemsChanged) return;
		ClearItemsGrid();
		ClearSelectionProperties();
		if (e.NewValue is not IEnumerable enumerable) return;
		UpdateItems(enumerable);
		RaiseSelectionChanged(SelectedIndex);
	}

	/// <summary>
	/// Current week changed event
	/// </summary>
	/// <param name="e">Argument for the event</param>
	protected void CurrentWeekChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.NewValue is not DateTime dateTime) return;
		ClearSelectionProperties();
		CreateWeek(dateTime);
		UpdateItems(Items);
		RaiseSelectionChanged(SelectedIndex);
	}

	/// <summary>
	/// Selected index changed event
	/// </summary>
	/// <param name="e">Argument for the event</param>
	void SelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (skipSelectedIndexChanged || e.NewValue is not int index) return;
		RaiseSelectionChanged(index);
	}

	/// <summary>
	/// Selected item changed event
	/// </summary>
	/// <param name="e">Argument for the event</param>
	void SelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (skipSelectedItemChanged || e.NewValue is not { } obj) return;
		RaiseSelectionChanged(obj);
	}

	/// <summary>
	/// Clears the selection properties
	/// </summary>
	void ClearSelectionProperties()
	{
		skipSelectedIndexChanged = true;
		SelectedIndex = -1;
		skipSelectedIndexChanged = false;
		skipSelectedItemChanged = true;
		SelectedItem = null;
		skipSelectedItemChanged = false;
	}

	/// <summary>
	/// Raises the selection changed event
	/// </summary>
	/// <param name="index">Index selected</param>
	void RaiseSelectionChanged(int index)
	{
		ClearSelection();
		var itemsGrid = this.FindControl<Grid>("ItemsGrid");
		var appointment = itemsGrid.GetLogicalDescendants().OfType<AppointmentControl>()
			.FirstOrDefault(x => x.Index == index);
		if (appointment != null)
			appointment.IsSelected = true;

		object? item = null;
		var list = GetItemsAsList();
		if (index >= 0 && index < list.Count)
			item = list[index];
		var eventArgs = new CalendarSelectionChangedEventArgs(SelectionChangedEvent)
		{ SelectedIndex = index, SelectedItem = item };
		RaiseEvent(eventArgs);
		skipSelectedItemChanged = true;
		SelectedItem = item;
		skipSelectedItemChanged = false;
	}

	/// <summary>
	/// Raises the selection changed event
	/// </summary>
	/// <param name="item">Item selected</param>
	void RaiseSelectionChanged(object item)
	{
		ClearSelection();
		var index = GetItemsAsList().IndexOf(item);

		var itemsGrid = this.FindControl<Grid>("ItemsGrid");
		var appointment = itemsGrid.GetLogicalDescendants().OfType<AppointmentControl>()
			.FirstOrDefault(x => x.Index == index);
		if (appointment != null)
			appointment.IsSelected = true;

		var eventArgs = new CalendarSelectionChangedEventArgs(SelectionChangedEvent)
		{ SelectedIndex = index, SelectedItem = item };
		RaiseEvent(eventArgs);
		skipSelectedIndexChanged = true;
		SelectedIndex = index;
		skipSelectedIndexChanged = false;
	}

	/// <summary>
	/// Updated the view with the given items
	/// </summary>
	/// <param name="enumerable">Items to process</param>
	void UpdateItems(IEnumerable enumerable)
	{
		var itemsGrid = this.FindControl<Grid>("ItemsGrid");
		var beginWeek = currentWeek.GetBeginWeek(FirstDayOfWeek);

		internalItems = Convert(enumerable);
		var weekList = internalItems.
		Where(x => x.GetFirstLogicalDescendant<IAppointmentControl>().IsInWeek(beginWeek)).
		OrderBy(x => x.GetFirstLogicalDescendant<IAppointmentControl>().Begin);
		for (var i = 0; i < daysPerWeek; i++)
		{
			if (itemsGrid.Children[i] is not Grid dayColumn) continue;

			var todayList = weekList.Where(x => x.GetFirstLogicalDescendant<IAppointmentControl>().IsInDay(beginWeek.AddDays(i))).ToList();
			AppointmentControlListHelper.ApplyIndentation(todayList);
			var rowDefinitions = new RowDefinitions();

			var previousEnd = double.NaN;
			var dayControls = new List<IControl?>();
			var j = 0;
			while (j < todayList.Count)
			{
				var item = todayList[j].GetFirstLogicalDescendant<IAppointmentControl>();
				var (begin, _) = item.GetFractionOfDay();
				AddEmptyRow(rowDefinitions, dayControls, previousEnd, begin);

				var appointmentGroup = GetAppointmentGroup(todayList, j);
				dayControls.Add(appointmentGroup.Control);
				rowDefinitions.Add(new RowDefinition(appointmentGroup.Length, GridUnitType.Star));
				previousEnd = appointmentGroup.Begin + appointmentGroup.Length;
				j = appointmentGroup.Index;
			}

			// Tail
			AddEmptyRowTail(rowDefinitions, dayControls, previousEnd);
			AddRows(dayColumn, rowDefinitions, dayControls);
		}
	}

	/// <summary>
	/// Get an appointment group from the given list
	/// </summary>
	/// <param name="list">List to get appointment from</param>
	/// <param name="beginIndex">Index to begin in</param>
	/// <returns>
	/// Index to continue in next iteration, the begin (fraction of day), the length (fraction of day) and the
	/// containing control
	/// </returns>
	(int Index, double Begin, double Length, IControl Control) GetAppointmentGroup(IList<IControl> list, int beginIndex)
	{
		var item = list[beginIndex].GetFirstLogicalDescendant<IAppointmentControl>();
		var (begin, _) = item.GetFractionOfDay();
		var count = AppointmentGroupHelper.GetGroupCount(list, beginIndex);
		var end = AppointmentGroupHelper.GetEnd(list, beginIndex, count);
		var length = end - begin;
		var indentationCount = AppointmentGroupHelper.GetIndentationCount(list, beginIndex, count);
		var grid = ControlFactory.CreateGrid(indentationCount);
		grid[!MarginProperty] = new Binding("AppointmentGroupMargin") { Source = this };

		for (var i = 0; i < indentationCount; i++)
		{
			if (grid.Children[i] is not Grid g)
				continue;
			var indentItems = AppointmentGroupHelper.GetIndentationItems(list, beginIndex, count, i);
			if (indentItems.Count == 0)
				continue;
			var groupControls = new List<IControl?>();
			var rowDefinitions = new RowDefinitions();
			var previous = double.NaN;
			foreach (var indentItem in indentItems)
			{
				item = indentItem.GetFirstLogicalDescendant<IAppointmentControl>();
				var (b, l) = item.GetFractionOfDay();
				// Within the group
				b = (b - begin) / length;
				l /= length;

				var emptyLength = b - (!double.IsNaN(previous) ? previous : 0.0d);
				if (emptyLength > 0.0d)
				{
					groupControls.Add(null);
					rowDefinitions.Add(new RowDefinition(emptyLength, GridUnitType.Star));
				}

				groupControls.Add(indentItem);
				rowDefinitions.Add(new RowDefinition(l, GridUnitType.Star));
				previous = b + l;
			}

			// Tail
			if (double.IsNaN(previous) || previous < 1.0d)
			{
				groupControls.Add(null);
				rowDefinitions.Add(new RowDefinition(1.0d - (!double.IsNaN(previous) ? previous : 0.0d), GridUnitType.Star));
			}

			ControlHelper.AddControlsToRows(g, groupControls, rowDefinitions);
		}

		return (beginIndex + count, begin, length, grid);
	}

	/// <summary>
	/// Adds an empty row for the tail (if applicable)
	/// </summary>
	/// <param name="rowDefinitions">List of row definitions to add to</param>
	/// <param name="controls">List of controls to add to</param>
	/// <param name="previousEnd">End of the previous appointment (as a fraction of the day)</param>
	static void AddEmptyRowTail(RowDefinitions rowDefinitions, ICollection<IControl?> controls, double previousEnd)
	{
		if (!double.IsNaN(previousEnd) && previousEnd >= 1.0d)
			return;

		controls.Add(null);
		rowDefinitions.Add(new RowDefinition(1.0d - (!double.IsNaN(previousEnd) ? previousEnd : 0.0d), GridUnitType.Star));
	}

	/// <summary>
	/// Adds an empty row (if applicable)
	/// </summary>
	/// <param name="rowDefinitions">List of row definitions to add to</param>
	/// <param name="controls">List of controls to add to</param>
	/// <param name="previousEnd">End of the previous appointment (as a fraction of the day)</param>
	/// <param name="begin">Begin of the current appointment (as a fraction of the day)</param>
	static void AddEmptyRow(RowDefinitions rowDefinitions, ICollection<IControl?> controls, double previousEnd,
		double begin)
	{
		var emptyLength = begin - (!double.IsNaN(previousEnd) ? previousEnd : 0.0d);
		if (emptyLength <= 0.0d)
			return;

		controls.Add(null);
		rowDefinitions.Add(new RowDefinition(emptyLength, GridUnitType.Star));
	}

	/// <summary>
	/// Adds controls to the given grid's rows
	/// </summary>
	/// <param name="grid">Grid to add to</param>
	/// <param name="rowDefinitions">List of row definitions</param>
	/// <param name="controls">Controls to adds</param>
	static void AddRows(Grid grid, RowDefinitions rowDefinitions, IReadOnlyList<IControl?> controls)
	{
		grid.RowDefinitions = rowDefinitions;
		grid.Children.Clear();
		foreach (var control in controls.Where(x => x != null))
			grid.Children.Add(control);

		for (var i = 0; i < controls.Count; i++)
		{
			if (controls[i] != null)
				Grid.SetRow(controls[i] as Control, i);
		}
	}

	/// <summary>
	/// Converts the given items to a handleable format
	/// </summary>
	/// <param name="enumerable">Items to process</param>
	/// <returns>Internal handleable format</returns>
	IList<IControl> Convert(IEnumerable enumerable)
	{
		var result = new List<IControl>();

		var i = 0;
		foreach (var e in enumerable)
		{
			if (ItemTemplate?.Build(e) is not { } p || !p.HasFirstLogicalDescendant<IAppointmentControl>()) continue;
			var item = p.GetFirstLogicalDescendant<IAppointmentControl>();
			item.Index = i;
			p.DataContext = e;
			result.Add(p);
			i++;
		}
		return result;
	}

	/// <summary>
	/// Clears the items grid
	/// </summary>
	void ClearItemsGrid()
	{
		var itemsGrid = this.FindControl<Grid>("ItemsGrid");
		itemsGrid.Children.Clear();
		var columnDefinitions = new ColumnDefinitions();
		for (var i = 0; i < daysPerWeek; i++)
		{
			var dayColumn = ControlFactory.CreateColumn();
			dayColumn.RowDefinitions = new RowDefinitions();
			columnDefinitions.Add(new ColumnDefinition(1.0d, GridUnitType.Star));
			itemsGrid.Children.Add(dayColumn);
			Grid.SetColumn(dayColumn, i);
		}

		itemsGrid.ColumnDefinitions = columnDefinitions;
	}

	/// <summary>
	/// Creates the week UI for the given current week
	/// </summary>
	/// <param name="week">The current week</param>
	void CreateWeek(DateTime week)
	{
		CreateHourTexts();
		CreateDayTexts(week);

		var weekGrid = this.FindControl<Grid>("WeekGrid");
		weekGrid.Children.Clear();

		var columnDefinitions = new ColumnDefinitions();
		var firstDayOfWeek = FirstDayOfWeek;
		for (var i = 0; i < daysPerWeek; i++)
		{
			var dayState = ControlFactory.CreateDayState(AddDay(firstDayOfWeek, i));
			var dayColumn = ControlFactory.CreateColumn();
			var rowDefinitions = new RowDefinitions();
			for (var j = 0; j < hoursPerDay; j++)
			{
				var hourCell = ControlFactory.CreateHourCell();
				dayColumn.Children.Add(hourCell);
				Grid.SetRow(hourCell, j);
				rowDefinitions.Add(new RowDefinition(1.0d, GridUnitType.Star));
			}

			dayColumn.RowDefinitions = rowDefinitions;
			columnDefinitions.Add(new ColumnDefinition(1.0d, GridUnitType.Star));
			weekGrid.Children.Add(dayState);
			weekGrid.Children.Add(dayColumn);
			Grid.SetColumn(dayColumn, i);
			Grid.SetColumn(dayState, i);
		}

		weekGrid.ColumnDefinitions = columnDefinitions;
		ClearItemsGrid();
	}

	/// <summary>
	/// Creates the day texts
	/// </summary>
	/// <param name="week">The current week</param>
	void CreateDayTexts(DateTime week)
	{
		var dayGrid = this.FindControl<Grid>("DayGrid");
		dayGrid.Children.Clear();
		var columnDefinitions = new ColumnDefinitions();
		var firstDayOfWeek = FirstDayOfWeek;
		var beginWeek = week.GetBeginWeek(firstDayOfWeek);
		for (var i = 0; i < daysPerWeek; i++)
		{
			var day = AddDay(firstDayOfWeek, i);
			var text = DateTimeHelper.DayOfWeekToString(day) + " " + beginWeek.AddDays(i).Day;
			var dayText = new TextBlock { Text = text };
			columnDefinitions.Add(new ColumnDefinition(1.0d, GridUnitType.Star));
			dayGrid.Children.Add(dayText);
			Grid.SetColumn(dayText, i);
		}

		dayGrid.ColumnDefinitions = columnDefinitions;
	}

	/// <summary>
	/// Creates the hour texts
	/// </summary>
	void CreateHourTexts()
	{
		var hourGrid = this.FindControl<Grid>("HourGrid");
		hourGrid.Children.Clear();
		var rowDefinitions = new RowDefinitions();
		for (var j = 0; j < hoursPerDay; j++)
		{
			var text = new DateTime(1970, 1, 1, j, 0, 0).ToShortTimeString();
			var hourText = new TextBlock { Text = text };
			hourGrid.Children.Add(hourText);
			Grid.SetRow(hourText, j);
			rowDefinitions.Add(new RowDefinition(1.0d, GridUnitType.Star));
		}

		hourGrid.RowDefinitions = rowDefinitions;
	}

	/// <summary>
	/// Adds a number of days to a day in the week
	/// </summary>
	/// <param name="dayOfWeek">Day of the week</param>
	/// <param name="i">Number of days to add</param>
	/// <returns>New day of week</returns>
	static DayOfWeek AddDay(DayOfWeek dayOfWeek, int i) => (DayOfWeek)(((int)dayOfWeek + i) % 7);

	/// <summary>Days per week</summary>
	const int daysPerWeek = 7;
	/// <summary>Hours per day</summary>
	const int hoursPerDay = 24;
	/// <summary>Allow delete</summary>
	bool allowDelete;
	/// <summary>Begin of the day</summary>
	TimeSpan beginOfTheDay = new(0, 0, 0);
	/// <summary>Current week</summary>
	DateTime currentWeek = DateTime.Now;
	/// <summary>End of the day</summary>
	TimeSpan endOfTheDay = new(0, 0, 0);
	/// <summary>Items</summary>
	IList<IControl> internalItems = new List<IControl>();
	/// <summary>Items</summary>
	IEnumerable items = new AvaloniaList<object>();
	/// <summary>State of the left mouse button</summary>
	bool leftButtonDown;
	/// <summary>Skip handling the items changed event flag</summary>
	bool skipItemsChanged;
	/// <summary>Skip handling the selected index changed event flag</summary>
	bool skipSelectedIndexChanged;
	/// <summary>Skip handling the selected item changed event flag</summary>
	bool skipSelectedItemChanged;
}