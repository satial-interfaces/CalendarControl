using Avalonia;
using Avalonia.ReactiveUI;
using SampleApp;

AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace().UseReactiveUI().StartWithClassicDesktopLifetime(args);