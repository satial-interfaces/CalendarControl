# CalendarControl for Avalonia

This is a calendar control (week view) for Avalonia. See and run the sample app to get to know it.

![CalendarControl screenshot](/Images/CalendarControl.png)

## How to use

First add a style to your App.axaml

````Xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:f="using:FluentAvalonia.Styling">
    <Application.Styles>
        <f:FluentAvaloniaTheme />
        <StyleInclude Source="avares://SampleApp/Themes/FluentCalendarControl.axaml" />
    </Application.Styles>
</Application>
````

Or use the default one

````Xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:f="using:FluentAvalonia.Styling">
    <Application.Styles>
        <f:FluentAvaloniaTheme />
        <StyleInclude Source="avares://CalendarControl/Themes/Default.axaml" />
    </Application.Styles>
</Application>
````

Then add the control to your Window.axaml

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
