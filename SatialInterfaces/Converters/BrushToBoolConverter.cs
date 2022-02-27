using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace SatialInterfaces.Converters;

/// <summary>
/// Solid color brush to boolean converter. Returns true if not transparent and false otherwise.
/// </summary>
public class BrushToBoolConverter : IValueConverter
{
	/// <inheritdoc />
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is not ISolidColorBrush b || targetType != typeof(bool))
			return BindingOperations.DoNothing;

		return b.Color.A <= 0;
	}

	/// <inheritdoc />
	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => BindingOperations.DoNothing;
}