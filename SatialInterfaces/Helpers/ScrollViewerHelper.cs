using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace SatialInterfaces.Helpers;

/// <summary>
/// This class contains ScrollViewer methods.
/// </summary>
public static class ScrollViewerHelper
{
	/// <summary>
	/// Scrolls the given position without any binding
	/// </summary>
	/// <param name="scrollViewer">Scroll viewer to scroll</param>
	/// <param name="newPosition">New position to scroll</param>
	public static void ScrollWithoutBinding(this ScrollViewer scrollViewer, Vector newPosition)
	{
		var indexerBinding = scrollViewer[!ScrollViewer.OffsetProperty];
		scrollViewer[!ScrollViewer.OffsetProperty] = emptyBinding;
		scrollViewer.Offset = newPosition;
		scrollViewer[!ScrollViewer.OffsetProperty] = indexerBinding;
	}

	/// <summary>
	/// Empty binding
	/// </summary>
	static readonly Binding emptyBinding = new();
}
