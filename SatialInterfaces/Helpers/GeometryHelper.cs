using Avalonia;

namespace SatialInterfaces.Helpers;

/// <summary>
/// This class contains geometry methods.
/// </summary>
public static class GeometryHelper
{
	/// <summary>
	/// Check if a X and Y value is in a rectangle
	/// </summary>
	/// <param name="x">X value</param>
	/// <param name="y">Y value</param>
	/// <param name="rect">Rectangle</param>
	/// <returns>True in there and false otherwise</returns>
	public static bool IsInRect(double x, double y, Rect rect)
	{
		var xInRect = x >= rect.Left && x < rect.Right;
		var yInRect = y >= rect.Top && y < rect.Bottom;
		return xInRect && yInRect;
	}
}