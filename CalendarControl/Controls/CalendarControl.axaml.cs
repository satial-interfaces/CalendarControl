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

namespace CalendarControl;

/// <summary>
/// This class represents a calendar control (week view)
/// </summary>
public class CalendarControl : UserControl
{
    /// <summary>First day of the week property</summary>
    public static readonly StyledProperty<DayOfWeek> FirstDayOfWeekProperty = AvaloniaProperty.Register<CalendarControl, DayOfWeek>(nameof(FirstDayOfWeek), DateTimeHelper.GetCurrentDateFormat().FirstDayOfWeek);
    /// <summary>Items property</summary>
    public static readonly DirectProperty<CalendarControl, IEnumerable> ItemsProperty = AvaloniaProperty.RegisterDirect<CalendarControl, IEnumerable>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
    /// <summary>Current week property</summary>
    public static readonly DirectProperty<CalendarControl, DateTime> CurrentWeekProperty = AvaloniaProperty.RegisterDirect<CalendarControl, DateTime>(nameof(CurrentWeek), o => o.CurrentWeek, (o, v) => o.CurrentWeek = v);
    /// <summary>The selected index property</summary>
    public static readonly StyledProperty<int> SelectedIndexProperty = AvaloniaProperty.Register<CalendarControl, int>(nameof(SelectedIndex), -1);
    /// <summary>The selection changed event</summary>
    public static readonly RoutedEvent<CalendarSelectionChangedEventArgs> SelectionChangedEvent = RoutedEvent.Register<CalendarControl, CalendarSelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

    /// <summary>
    /// Initializes static members of the <see cref="CalendarControl"/> class.
    /// </summary>
    static CalendarControl()
    {
        ItemsProperty.Changed.AddClassHandler<CalendarControl>((x, e) => x.ItemsChanged(e));
        CurrentWeekProperty.Changed.AddClassHandler<CalendarControl>((x, e) => x.CurrentWeekChanged(e));
    }

	/// <summary>
	/// Initializes a new instance of the <see cref="CalendarControl" /> class.
	/// </summary>
    public CalendarControl()
    {
        AvaloniaXamlLoader.Load(this);
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

        int index;
        if (e.Pointer.Captured is AppointmentControl appointment)
        {
            appointment.IsSelected = true;
            ClearSelection(appointment);
            index = appointment.Index;
        }
        else
        {
            ClearSelection(null);
            index = -1;
        }

        leftButtonDown = false;
        base.OnPointerReleased(e);
        SelectedIndex = index;
        var eventArgs = new CalendarSelectionChangedEventArgs(SelectionChangedEvent) { SelectedIndex = index };
        RaiseEvent(eventArgs);
    }

    /// <summary>
    /// Clear the selection bit for the other appointments.
    /// </summary>
    /// <param name="skip">Appointment to skip</param>
    void ClearSelection(AppointmentControl? skip)
    {
        var itemsGrid = this.FindControl<Grid>("ItemsGrid");
        var appointments = itemsGrid.GetLogicalDescendants().OfType<AppointmentControl>();
        foreach (var appointment in appointments)
        {
            if (appointment == skip) continue;
            appointment.IsSelected = false;
        }
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
    /// Updated the view with the given items
    /// </summary>
    /// <param name="items">Items to process</param>
    void UpdateItems(IEnumerable items)
    {
        var itemsGrid = this.FindControl<Grid>("ItemsGrid");

        var beginWeek = currentWeek.GetBeginWeek(FirstDayOfWeek);

        var weekList = Convert(items).Where(x => x.IsInCurrentWeek(beginWeek)).OrderBy(x => x.Begin);
        for (var i = 0; i < daysPerWeek; i++)
        {
            if (itemsGrid.Children[i] is not Grid dayColumn) continue;

            var todayList = weekList.Where(x => x.IsInDay(beginWeek.AddDays(i))).ToList();
            AppointmentItemListHelper.ApplyIndentation(todayList);
            var rowDefinitions = new RowDefinitions();

            var previous = double.NaN;
            var dayControls = new List<IControl?>();
            var j = 0;
            while (j < todayList.Count)
            {
                var (begin, _) = todayList[j].GetFractionOfDay();

                var emptyLength = begin - (!double.IsNaN(previous) ? previous : 0.0d);
                if (emptyLength > 0.0d)
                {
                    dayControls.Add(null);
                    rowDefinitions.Add(new RowDefinition(emptyLength, GridUnitType.Star));
                }

                var appointmentGroup = GetAppointmentGroup(todayList, j);
                dayControls.Add(appointmentGroup.Control);
                rowDefinitions.Add(new RowDefinition(appointmentGroup.Length, GridUnitType.Star));
                previous = appointmentGroup.Begin + appointmentGroup.Length;
                j = appointmentGroup.Index;
            }

            // Tail
            if (double.IsNaN(previous) || previous < 1.0d)
            {
                dayControls.Add(null);
                rowDefinitions.Add(new RowDefinition(1.0d - (!double.IsNaN(previous) ? previous : 0.0d), GridUnitType.Star));
            }

            dayColumn.RowDefinitions = rowDefinitions;
            dayColumn.Children.Clear();
            foreach (var dayControl in dayControls.Where(x => x != null))
                dayColumn.Children.Add(dayControl);

            for (var k = 0; k < dayControls.Count; k++)
            {
                if (dayControls[k] != null)
                    Grid.SetRow(dayControls[k] as Control, k);
            }
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
    IEnumerable<AppointmentItem> Convert(IEnumerable enumerable)
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

        if (begin == null)
            return result;
        if (end == null)
            return result;
        if (text == null)
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
            var rowDefinitions = new RowDefinitions();
            dayColumn.RowDefinitions = rowDefinitions;
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
        for (var i = 0; i < daysPerWeek; i++)
        {
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
            weekGrid.Children.Add(dayColumn);
            Grid.SetColumn(dayColumn, i);
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
    IEnumerable items = new AvaloniaList<object>();
    /// <summary>
    /// State of the left mouse button
    /// </summary>
    bool leftButtonDown;
    /// <summary>First day of the week property</summary>
	public DayOfWeek FirstDayOfWeek { get => GetValue(FirstDayOfWeekProperty); set => SetValue(FirstDayOfWeekProperty, value); }
    /// <summary>Items property</summary>
    public IEnumerable Items { get => items; set => SetAndRaise(ItemsProperty, ref items, value); }
    /// <summary>Current week property</summary>
	public DateTime CurrentWeek { get => currentWeek; set => SetAndRaise(CurrentWeekProperty, ref currentWeek, value); }
    /// <summary>Item definition</summary>
    public ObservableCollection<CalendarControlItemTemplate> ItemTemplate { get; } = new();
    /// <summary>Selected index</summary>
	public int SelectedIndex { get => GetValue(SelectedIndexProperty); set => SetValue(SelectedIndexProperty, value); }
    /// <summary>Occurs when selection changed</summary>
    public event EventHandler<CalendarSelectionChangedEventArgs> SelectionChanged { add => AddHandler(SelectionChangedEvent, value); remove => RemoveHandler(SelectionChangedEvent, value); }
}
