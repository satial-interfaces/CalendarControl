using System;
using System.Globalization;

namespace CalendarControl.Helpers{

/// <summary>
/// Date/time helper class: it provides methods and extension methods
/// </summary>
internal static class DateTimeHelper
{
	/// <summary>
	/// The weekday/weekend state for a given day.
	/// </summary>
	public enum DayState
	{
		/// <summary>
		/// A work day.
		/// </summary>
		WorkDay,
		/// <summary>
		/// A weekend.
		/// </summary>
		Weekend,
		/// <summary>
		/// Morning is a workday, afternoon is the start of the weekend.
		/// </summary>
		WorkdayMorning
	}

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

	/// <summary>
	/// Return if the passed in day of the week is a weekend. note: state pulled from http://en.wikipedia.org/wiki/Workweek_and_weekend
	/// </summary>
	/// <param name="obj">The CultureInfo this object.</param>
	/// <param name="dayOfWeek">The Day of the week to return the state of.</param>
	/// <returns>The weekday/weekend state of the passed in day of the week.</returns>
	public static DayState GetDayState(CultureInfo obj, DayOfWeek dayOfWeek)
    {
        var items = obj.Name.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (items.Length < 1) return DefaultDayState(dayOfWeek);
        switch (items[^1])
        {
            case "DZ": // Algeria
            case "BH": // Bahrain
            case "BD": // Bangladesh
            case "EG": // Egypt
            case "IQ": // Iraq
            case "IL": // Israel
            case "JO": // Jordan
            case "KW": // Kuwait
            case "LY": // Libya
                       // Northern Malaysia (only in the states of Kelantan, Terengganu, and Kedah)
            case "MV": // Maldives
            case "MR": // Mauritania
            case "NP": // Nepal
            case "OM": // Oman
            case "QA": // Qatar
            case "SA": // Saudi Arabia
            case "SD": // Sudan
            case "SY": // Syria
            case "AE": // U.A.E.
            case "YE": // Yemen
                return dayOfWeek is DayOfWeek.Thursday or DayOfWeek.Friday
                    ? DayState.Weekend
                    : DayState.WorkDay;

            case "AF": // Afghanistan
            case "IR": // Iran
                if (dayOfWeek == DayOfWeek.Thursday)
                    return DayState.WorkdayMorning;
                return dayOfWeek == DayOfWeek.Friday ? DayState.Weekend : DayState.WorkDay;

            case "BN": // Brunei Darussalam
                return dayOfWeek is DayOfWeek.Friday or DayOfWeek.Sunday
                    ? DayState.Weekend
                    : DayState.WorkDay;

            case "MX": // Mexico
            case "TH": // Thailand
                if (dayOfWeek == DayOfWeek.Saturday)
                    return DayState.WorkdayMorning;
                return dayOfWeek == DayOfWeek.Sunday
                    ? DayState.Weekend
                    : DayState.WorkDay;
        }

        // most common Saturday/Sunday
        return DefaultDayState(dayOfWeek);
    }

	/// <summary>
	/// Gets the default day stete
	/// </summary>
	/// <param name="dayOfWeek">Day of the week to check</param>
	/// <returns>The day state</returns>
    static DayState DefaultDayState(DayOfWeek dayOfWeek)
    {
        return dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday ? DayState.Weekend : DayState.WorkDay;
    }
}
}
