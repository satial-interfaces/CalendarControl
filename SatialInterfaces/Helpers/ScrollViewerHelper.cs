using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace SatialInterfaces.Helpers;

/// <summary>This class contains ScrollViewer methods.</summary>
public static class ScrollViewerHelper
{
	/// <summary>
	/// Scrolls the given position without any binding
	/// </summary>
	/// <param name="scrollViewer">Scroll viewer to scroll</param>
	/// <param name="offset">Offset to scroll</param>
	public static void ScrollWithoutBinding(this ScrollViewer scrollViewer, Vector offset)
	{
		var indexerBinding = scrollViewer[!ScrollViewer.OffsetProperty];
		scrollViewer[!ScrollViewer.OffsetProperty] = EmptyBinding;
		scrollViewer.Offset = offset;
		scrollViewer[!ScrollViewer.OffsetProperty] = indexerBinding;
	}

	/// <summary>
	/// Empty binding
	/// </summary>
	static readonly Binding EmptyBinding = new();
}
