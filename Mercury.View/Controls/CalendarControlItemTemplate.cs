using Avalonia;
using Avalonia.Data;

namespace Mercury.View;

/// <summary>
/// This class contains the base (abstract) for the item template.
/// </summary>
public abstract class CalendarControlItemTemplate : AvaloniaObject
{
    [AssignBinding]
    /// <summary>
    /// Binding provided by the item defintion
    /// </summary>
    public virtual IBinding? Binding { get; set; }
}

/// <summary>
/// This class represents the Begin item defintion
/// </summary>
public class BeginItem : CalendarControlItemTemplate { }

/// <summary>
/// This class represents the End item defintion
/// </summary>
public class EndItem : CalendarControlItemTemplate { }

/// <summary>
/// This class represents the Text item defintion
/// </summary>
public class TextItem : CalendarControlItemTemplate { }
/// <summary>
/// This class represents the Text item defintion
/// </summary>
public class ColorItem : CalendarControlItemTemplate { }