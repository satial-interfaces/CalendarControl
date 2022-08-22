using Avalonia;

namespace SatialInterfaces.Helpers;

/// <summary>This class contains geometry methods.</summary>
public static class GeometryHelper
{
	/// <summary>
	/// Check if a vector is in a rectangle
	/// </summary>
	/// <param name="v">X value</param>
	/// <param name="rect">Rectangle</param>
	/// <returns>True in there and false otherwise</returns>
	public static bool IsInRect(Vector v, Rect rect)
	{
		var (x, y) = v;
		var xInRect = x >= rect.Left && x < rect.Right;
		var yInRect = y >= rect.Top && y < rect.Bottom;
		return xInRect && yInRect;
	}
}
