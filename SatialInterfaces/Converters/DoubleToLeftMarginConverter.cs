using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace SatialInterfaces.Converters;

/// <summary>
/// Double (width) to Margin.Left converter
/// </summary>
internal class DoubleToLeftMarginConverter : IValueConverter
{
    /// <inheritdoc />
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not double d || targetType != typeof(Thickness))
            return BindingOperations.DoNothing;

        return new Thickness(d, 0.0d, 0.0d, 0.0d);
    }

    /// <inheritdoc />
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}