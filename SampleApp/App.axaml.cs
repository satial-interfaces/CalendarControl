using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace SampleApp;

public class App : Application
{
	public override void Initialize()
	{
		AvaloniaXamlLoader.Load(this);
	}

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
			desktop.MainWindow = new MainWindow();
		// var theme = AvaloniaLocator.Current.GetService<FluentAvalonia.Styling.FluentAvaloniaTheme>();
		// if (theme != null)
		//     theme.RequestedTheme = "Dark";

		base.OnFrameworkInitializationCompleted();
	}
}