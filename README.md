# CalendarControl for Avalonia

This is a calendar control (week view) for Avalonia. See and run the sample app to get to know it.

![CalendarControl screenshot](/Images/CalendarControl.png)

## How to use

First add the package to your project. Use NuGet to get it: https://www.nuget.org/packages/CalendarControl.Avalonia/

Or use this commands in the Package Manager console to install the package manually
```
Install-Package CalendarControl.Avalonia
```

Second add a style to your App.axaml (from the sample app)

````Xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Styles>
        <!-- Overall style goes here: <FluentTheme Mode="Light"/> -->
        <StyleInclude Source="avares://SampleApp/Themes/FluentCalendarControl.axaml" />
    </Application.Styles>
</Application>
````

Or use the default one

````Xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Styles>
        <!-- Overall style goes here: <FluentTheme Mode="Light"/> -->
        <StyleInclude Source="avares://CalendarControl/Themes/Default.axaml" />
    </Application.Styles>
</Application>
````

Then add the control to your Window.axaml (minimum)

````Xml
<Window xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:si="clr-namespace:SatialInterfaces.Controls;assembly=CalendarControl">
    <Grid>
        <si:CalendarControl />
    </Grid>
</Window>
````

It's even better to specify the item template with binding to your view model

````Xml
<Window xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:si="clr-namespace:SatialInterfaces.Controls;assembly=CalendarControl">
    <Grid>
        <si:CalendarControl>
            <si:CalendarControl.ItemTemplate>
                <DataTemplate>
                    <si:AppointmentControl Begin="{Binding Begin}" End="{Binding End}" Text="{Binding Text}" />
                </DataTemplate>
            </si:CalendarControl.ItemTemplate>
        </si:CalendarControl>
    </Grid>
</Window>
````
