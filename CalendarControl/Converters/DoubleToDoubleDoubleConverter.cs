using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace CalendarControl.Converters;

/// <summary>
/// Double (height) to 2X converter
/// </summary>
class DoubleToDoubleDoubleConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double d || targetType != typeof(double))
            return BindingOperations.DoNothing;

        return d * 2.0d;
    }
    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}
