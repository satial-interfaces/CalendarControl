using Avalonia;
using Avalonia.Data;

namespace Mercury.View;

/// <summary>
/// This class contains the base (abstract) for the item template.
/// </summary>
public abstract class CalendarControlItemTemplate : AvaloniaObject
{
    /// <summary>
    /// Binding provided by the item defintion
    /// </summary>
    [AssignBinding]
    public IBinding? Binding { get; set; }
}

/// <summary>
/// This class represents the Begin item definition
/// </summary>
public class BeginItem : CalendarControlItemTemplate { }

/// <summary>
/// This class represents the End item definition
/// </summary>
public class EndItem : CalendarControlItemTemplate { }

/// <summary>
/// This class represents the Text item definition
/// </summary>
public class TextItem : CalendarControlItemTemplate { }
/// <summary>
/// This class represents the Text item definition
/// </summary>
public class ColorItem : CalendarControlItemTemplate { }