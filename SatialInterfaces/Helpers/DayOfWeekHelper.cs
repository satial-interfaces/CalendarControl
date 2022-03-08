using System;

namespace SatialInterfaces.Helpers;

/// <summary>
/// This class contains day of week helper methods
/// </summary>
public static class DayOfWeekHelper
{
	/// <summary>
	/// Adds a number of days to a day in the week
	/// </summary>
	/// <param name="dayOfWeek">Day of the week</param>
	/// <param name="i">Number of days to add</param>
	/// <returns>New day of week</returns>
	public static DayOfWeek AddDay(DayOfWeek dayOfWeek, int i) => (DayOfWeek)(((int)dayOfWeek + i) % 7);
}
