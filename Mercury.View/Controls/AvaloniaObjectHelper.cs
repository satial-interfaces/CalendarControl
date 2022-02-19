using System;
using Avalonia;
using Avalonia.Data;

namespace Mercury.View;

/// <summary>
/// This class provides avalonia object functions
/// </summary>
public static class AvaloniaObjectHelper
{
    /// <summary>
    /// Get an observable value
    /// </summary>
    /// <param name="target">Target to get value from</param>
    /// <param name="binding">Specific bidning</param>
    /// <param name="source">Source value</param>
    /// <param name="defaultValue">Default value to use if not fetched</param>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <returns>The value or default value otherwise</returns>
    public static T GetObservableValue<T>(this IAvaloniaObject target, IBinding binding, object source, T defaultValue)
    {
        if (binding is not Binding bind) return defaultValue;
        bind.Source = source;
        var instancedBinding = bind.Initiate(target, null);
        if (instancedBinding == null || instancedBinding.Observable == null) return defaultValue;
        T result = defaultValue;

        instancedBinding.Observable.Subscribe(x =>
        {
            if (x != null) result = (T)x;
        });
        return result;
    }
}