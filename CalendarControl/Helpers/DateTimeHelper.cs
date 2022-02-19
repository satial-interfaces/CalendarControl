using System;
using System.Globalization;

namespace CalendarControl.Helpers;

/// <summary>
/// Date/time helper class: it provides methods and extension methods
/// </summary>
internal static class DateTimeHelper
{
    /// <summary>
    /// Gets the begin of the week
    /// </summary>
    /// <param name="obj">Object to act on</param>
    /// <param name="firstDayOfWeek">First day of the week constant</param>
    /// <returns>Begin of the week</returns>
    public static DateTime GetBeginWeek(this DateTime obj, DayOfWeek firstDayOfWeek)
    {
        var dayOfWeek = (int)obj.DayOfWeek;
        var diff = (int)firstDayOfWeek - dayOfWeek;
        var begin = diff <= 0 ? obj.AddDays(diff) : obj.AddDays(diff - 7);
        return begin.Date;
    }

    /// <summary>
    /// Gets the end of the week
    /// </summary>
    /// <param name="obj">Object to act on</param>
    /// <param name="firstDayOfWeek">First day of the week constant</param>
    /// <returns>End of the week</returns>
    public static DateTime GetEndWeek(this DateTime obj, DayOfWeek firstDayOfWeek)
    {
        var end = GetBeginWeek(obj, firstDayOfWeek).AddDays(7).AddTicks(-1);
        return end;
    }

    /// <summary>
    /// Gets the current date time format
    /// </summary>
    /// <returns>Date time format</returns>
    public static DateTimeFormatInfo GetCurrentDateFormat()
    {
        if (CultureInfo.CurrentCulture.Calendar is GregorianCalendar)
            return CultureInfo.CurrentCulture.DateTimeFormat;

        DateTimeFormatInfo dateTimeFormat;
        foreach (var calendar in CultureInfo.CurrentCulture.OptionalCalendars)
        {
            if (calendar is not GregorianCalendar) continue;
            dateTimeFormat = new CultureInfo(CultureInfo.CurrentCulture.Name).DateTimeFormat;
            dateTimeFormat.Calendar = calendar;
            return dateTimeFormat;
        }

        // if there are no GregorianCalendars in the OptionalCalendars
        // list, use the invariant date time format
        dateTimeFormat = new CultureInfo(CultureInfo.InvariantCulture.Name).DateTimeFormat;
        dateTimeFormat.Calendar = new GregorianCalendar();
        return dateTimeFormat;
    }

    /// <summary>
    /// Converts the given day of the week to a string
    /// </summary>
    /// <param name="dayOfWeek">Day of the week to convert</param>
    /// <returns>The string</returns>
    public static string DayOfWeekToString(DayOfWeek dayOfWeek) => GetCurrentDateFormat().DayNames[(int)dayOfWeek];
}