using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace SatialInterfaces.Converters;

/// <summary>
/// This class represents a one-way converter.
/// </summary>
internal class OneWayConverter<TSource, TTarget> : IValueConverter
{
	/// <summary>
	/// Gets the instance of this class
	/// </summary>
	/// <param name="convert">Convert function to pass</param>
	/// <returns>An instance of this class</returns>
	public static OneWayConverter<TSource, TTarget> GetInstance(Func<TSource, object?, CultureInfo, TTarget> convert) => new(convert);

	/// <summary>
	/// Initializes a new instance of the <see cref="SatialInterfaces.Converters.OneWayConverter" /> class.
	/// </summary>
	/// <param name="convert">Convert function to pass</param>
	public OneWayConverter(Func<TSource, object?, CultureInfo, TTarget> convert) => this.convert = convert;

	/// <inheritdoc />
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if ((typeof(TSource).IsValueType && value == null) || typeof(TTarget) != targetType)
			return BindingOperations.DoNothing;
		return convert((TSource)value!, parameter, culture);
	}

	/// <inheritdoc />
	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => BindingOperations.DoNothing;

	/// <summary>
	/// Convert method reference.
	/// </summary>
	readonly Func<TSource, object?, CultureInfo, TTarget> convert;
}
