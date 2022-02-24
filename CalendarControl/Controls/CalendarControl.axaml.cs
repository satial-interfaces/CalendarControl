using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using CalendarControl.Factories;
using CalendarControl.Helpers;

namespace CalendarControl.Controls;

/// <summary>
/// This class represents a calendar control (week view)
/// </summary>
public class CalendarControl : ContentControl, IStyleable
{
    /// <inheritdoc />
    Type IStyleable.StyleKey => typeof(ContentControl);

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
    /// <summary>The selected index property</summary>
    public static readonly StyledProperty<int> SelectedIndexProperty = AvaloniaProperty.Register<CalendarControl, int>(nameof(SelectedIndex), -1);
    /// <summary>The selected item property</summary>
    public static readonly StyledProperty<object> SelectedItemProperty = AvaloniaProperty.Register<CalendarControl, object>(nameof(SelectedItem));
    /// <summary>The selection changed event</summary>
    public static readonly RoutedEvent<CalendarSelectionChangedEventArgs> SelectionChangedEvent = RoutedEvent.Register<CalendarControl, CalendarSelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);
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
    /// <summary>Item definition</summary>
    public ObservableCollection<CalendarControlItemTemplate> ItemTemplate { get; } = new();
    /// <summary>Selected index</summary>
	public int SelectedIndex { get => GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }
    /// <summary>Selected item</summary>
	public object SelectedItem { get => GetValue(SelectedItemProperty); set => SetValue(SelectedItemProperty, value); }
    /// <summary>Occurs when selection changed</summary>
    public event EventHandler<CalendarSelectionChangedEventArgs> SelectionChanged { add => AddHandler(SelectionChangedEvent, value); remove => RemoveHandler(SelectionChangedEvent, value); }
    /// <summary>
    /// Initializes static members of the <see cref="CalendarControl"/> class.
    /// </summary>
    static CalendarControl()
    {
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
        var index = items.ToList().IndexOf(item);
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

        ClearSelection();
        var index = e.Pointer.Captured is AppointmentControl appointment ? appointment.Index : -1;
        leftButtonDown = false;
        base.OnPointerReleased(e);
        SelectedIndex = index;
    }

    /// <summary>
    /// Scroll viewer bounds changed: adjust scrollable grid as well
    /// </summary>
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
    void ScrollIntoView(IList<AppointmentItem> list, int index)
    {
        CurrentWeek = list[index].Begin;

        var scrollViewer = this.FindControl<ScrollViewer>("ScrollViewer");
        var scrollViewerRect = scrollViewer.Bounds;

        var begin = (list[index].Begin - list[index].Begin.Date).TotalDays;
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
        ClearItemsGrid();
        if (e.NewValue is not IEnumerable enumerable) return;
        UpdateItems(enumerable);
    }

    /// <summary>
    /// Current week changed event
    /// </summary>
    /// <param name="e">Argument for the event</param>
    protected void CurrentWeekChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not DateTime dateTime) return;
        CreateWeek(dateTime);
        UpdateItems(Items);
    }

    /// <summary>
    /// Selected index changed event
    /// </summary>
    /// <param name="e">Argument for the event</param>
    void SelectedIndexChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not int index) return;

        var itemsGrid = this.FindControl<Grid>("ItemsGrid");
        var appointment = itemsGrid.GetLogicalDescendants().OfType<AppointmentControl>().FirstOrDefault(x => x.Index == index);
        if (appointment != null)
            appointment.IsSelected = true;

        object? item = null;
        var list = items.ToList();
        if (index >= 0 && index < list.Count)
            item = list[index];
        var eventArgs = new CalendarSelectionChangedEventArgs(SelectionChangedEvent) { SelectedIndex = index, SelectedItem = item };
        RaiseEvent(eventArgs);
    }

    /// <summary>
    /// Selected item changed event
    /// </summary>
    /// <param name="e">Argument for the event</param>
    void SelectedItemChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.NewValue is not object obj) return;
        var index = Items.ToList().FindIndex(x => x == obj);

        var itemsGrid = this.FindControl<Grid>("ItemsGrid");
        var appointment = itemsGrid.GetLogicalDescendants().OfType<AppointmentControl>().FirstOrDefault(x => x.Index == index);
        if (appointment != null)
            appointment.IsSelected = true;

        var eventArgs = new CalendarSelectionChangedEventArgs(SelectionChangedEvent) { SelectedIndex = index, SelectedItem = obj };
        RaiseEvent(eventArgs);
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
        var weekList = internalItems.Where(x => x.IsInCurrentWeek(beginWeek)).OrderBy(x => x.Begin);
        for (var i = 0; i < daysPerWeek; i++)
        {
            if (itemsGrid.Children[i] is not Grid dayColumn) continue;

            var todayList = weekList.Where(x => x.IsInDay(beginWeek.AddDays(i))).ToList();
            AppointmentItemListHelper.ApplyIndentation(todayList);
            var rowDefinitions = new RowDefinitions();

            var previousEnd = double.NaN;
            var dayControls = new List<IControl?>();
            var j = 0; while (j < todayList.Count)
            {
                var (begin, _) = todayList[j].GetFractionOfDay();
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
    static void AddEmptyRow(RowDefinitions rowDefinitions, ICollection<IControl?> controls, double previousEnd, double begin)
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
    /// Get an appointment group from the given list
    /// </summary>
    /// <param name="list">List to get appointment from</param>
    /// <param name="beginIndex">Index to begin in</param>
    /// <returns>Index to continue in next iteration, the begin (fraction of day), the length (fraction of day) and the containing control</returns>
    static (int Index, double Begin, double Length, IControl Control) GetAppointmentGroup(IList<AppointmentItem> list, int beginIndex)
    {
        var (begin, _) = list[beginIndex].GetFractionOfDay();
        var count = AppointmentGroupHelper.GetGroupCount(list, beginIndex);
        var end = AppointmentGroupHelper.GetEnd(list, beginIndex, count);
        var length = end - begin;
        var indentationCount = AppointmentGroupHelper.GetIndentationCount(list, beginIndex, count);
        var grid = ControlFactory.CreateGrid(indentationCount);

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
                var (b, l) = indentItem.GetFractionOfDay();
                // Within the group
                b = (b - begin) / length;
                l /= length;

                var emptyLength = b - (!double.IsNaN(previous) ? previous : 0.0d);
                if (emptyLength > 0.0d)
                {
                    groupControls.Add(null);
                    rowDefinitions.Add(new RowDefinition(emptyLength, GridUnitType.Star));
                }

                groupControls.Add(ControlFactory.CreateAppointment(indentItem.Begin, indentItem.Text, indentItem.Color, indentItem.Index));
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
    /// Converts the given items to a handleable format
    /// </summary>
    /// <param name="enumerable">Items to process</param>
    /// <returns>Internal handleable format</returns>
    IList<AppointmentItem> Convert(IEnumerable enumerable)
    {
        var result = new List<AppointmentItem>();

        var enumerator = enumerable.GetEnumerator();
        if (!enumerator.MoveNext())
            return result;

        var i = 0;
        var obj = enumerator.Current;
        if (obj == null) return result;

        var begin = GetBinding<BeginItem>();
        var end = GetBinding<EndItem>();
        var text = GetBinding<TextItem>();
        var color = GetBinding<ColorItem>();

        if (begin == null || end == null || text == null)
            return result;
        var item = CreateItem(obj, begin, end, text, color, i);
        if (item == null)
            return result;
        if (item.IsValid())
            result.Add(item);

        while (enumerator.MoveNext())
        {
            i++;
            obj = enumerator.Current;
            if (obj == null)
                return new List<AppointmentItem>();
            item = CreateItem(obj, begin, end, text, color, i);
            if (item == null)
                return new List<AppointmentItem>();
            if (item.IsValid())
                result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// Gets the binding for the given calendar control item template
    /// </summary>
    /// <typeparam name="T">Type of the calendar control item template</typeparam>
    /// <returns>The binding or null otherwise</returns>
    IBinding? GetBinding<T>() where T : CalendarControlItemTemplate => ItemTemplate.FirstOrDefault(x => x.GetType() == typeof(T))?.Binding;

    /// <summary>
    /// Creates a calendar control item using the given bindings
    /// </summary>
    /// <param name="obj">Source object</param>
    /// <param name="beginBinding">Binding containing the begin</param>
    /// <param name="endBinding">Binding containing the end</param>
    /// <param name="textBinding">Binding containing the text</param>
    /// <param name="colorBinding">Binding containing the color</param>
    /// <param name="index">Index to use for the calendar control item</param>
    /// <returns>The calendar control item or null otherwise</returns>
    AppointmentItem? CreateItem(object obj, IBinding beginBinding, IBinding endBinding, IBinding textBinding, IBinding? colorBinding, int index)
    {
        var itemTemplate = ItemTemplate.FirstOrDefault(x => x.GetType() == typeof(BeginItem));
        if (itemTemplate == null) return null;
        var begin = itemTemplate.GetObservableValue(beginBinding, obj, DateTime.MinValue);

        itemTemplate = ItemTemplate.FirstOrDefault(x => x.GetType() == typeof(EndItem));
        if (itemTemplate == null) return null;
        var end = itemTemplate.GetObservableValue(endBinding, obj, DateTime.MinValue);

        itemTemplate = ItemTemplate.FirstOrDefault(x => x.GetType() == typeof(TextItem));
        if (itemTemplate == null) return null;
        var text = itemTemplate.GetObservableValue(textBinding, obj, "");

        itemTemplate = ItemTemplate.FirstOrDefault(x => x.GetType() == typeof(ColorItem));
        Color? color = null;
        if (itemTemplate != null && colorBinding != null)
            color = itemTemplate.GetObservableValue(colorBinding, obj, Colors.Transparent);

        return new AppointmentItem { Begin = begin, End = end, Text = text, Color = color ?? Colors.Transparent, Index = index };
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
    /// <summary>Current week</summary>
    DateTime currentWeek = DateTime.Now;
    /// <summary>Begin of the day</summary>
    TimeSpan beginOfTheDay = new(0, 0, 0);
    /// <summary>End of the day</summary>
    TimeSpan endOfTheDay = new(0, 0, 0);
    /// <summary>Items</summary>
    IEnumerable items = new AvaloniaList<object>();
    /// <summary>Items</summary>
    IList<AppointmentItem> internalItems = new List<AppointmentItem>();
    /// <summary>State of the left mouse button</summary>
    bool leftButtonDown;
}
