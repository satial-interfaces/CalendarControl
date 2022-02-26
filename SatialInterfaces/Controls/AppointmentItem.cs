using System;
using Avalonia.Media;

namespace SatialInterfaces.Controls;

/// <summary>
/// This class represents an item (appointment) from the calendar control
/// </summary>
public class AppointmentItem
{
    /// <summary>The begin</summary>
    public DateTime Begin { get; init; }

    /// <summary>The end</summary>
    public DateTime End { get; init; }

    /// <summary>The length</summary>
    public TimeSpan Length => End - Begin;

    /// <summary>The text</summary>
    public string Text { get; init; } = "";
    /// <summary>The color</summary>
    public Color Color { get; init; }
    /// <summary>The index in the list</summary>
    public int Index { get; init; }
    /// <summary>The indentation</summary>
    public int Indent { get => indent; set { if (value >= 0) indent = value; } }

    /// <summary>
    /// Gets the begin and length as a fraction of the day
    /// </summary>
    /// <returns>Begin and length as a fraction of the day</returns>
    public (double Begin, double Length) GetFractionOfDay()
    {
        var beginTimeSpan = Begin.TimeOfDay;
        var endTimeSpan = End.Subtract(Begin.Date);
        var begin = beginTimeSpan.TotalHours / 24.0d;
        var end = endTimeSpan.TotalHours / 24.0d;
        if (end > 1.0d)
            end = 1.0d;
        var length = end - begin;
        return (begin, length);
    }

    /// <summary>
    /// Checks if this instance is in the given day
    /// </summary>
    /// <param name="dateTime">date/time for the given day</param>
    /// <returns>True if it is and false otherwise</returns>
    public bool IsInDay(DateTime dateTime)
    {
        var beginDay = dateTime.Date;
        var endDay = beginDay.AddDays(1).AddTicks(-1);
        return Begin >= beginDay && Begin < endDay;
    }

    /// <summary>
    /// Checks if this instance is valid
    /// </summary>
    /// <returns>True if it is and false otherwise</returns>
    public bool IsValid() => End > Begin;

    /// <summary>
    /// Checks if this instance has overlap with another instance
    /// </summary>
    /// <param name="other">Other instance to check with</param>
    /// <returns>True if it has and false otherwise</returns>
    public bool HasOverlap(AppointmentItem other) => other.Begin >= Begin && other.Begin < End || Begin >= other.Begin && Begin < other.End;

    /// <summary>
    /// Check if a given item is in the given week.
    /// </summary>
    /// <param name="beginWeek">Begin of the week</param>
    /// <returns>True if it is and false otherwise</returns>
    public bool IsInCurrentWeek(DateTime beginWeek) => Begin >= beginWeek;

    /// <summary>
    /// The indentation
    /// </summary>
    int indent;
}