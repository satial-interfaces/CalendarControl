using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace SatialInterfaces.Controls;

/// <summary>
/// This class represents an appointment and extends Border by adding an index
/// </summary>
[PseudoClasses(":pressed", ":selected")]
public class AppointmentControl : Border, ISelectable
{
    /// <summary>The index property</summary>
    public static readonly StyledProperty<int> IndexProperty = AvaloniaProperty.Register<AppointmentControl, int>(nameof(Index), -1);

    /// <summary>The index</summary>
    public int Index
    {
        get => GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    /// <inheritdoc />
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            isSelected = value;
            PseudoClasses.Set(":selected", isSelected);
        }
    }

    /// <summary>
    /// Is selected
    /// </summary>
    bool isSelected;
}